using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ByteSizeLib;
using Serilog;

namespace Deployer.FileSystem.Gpt
{
    public class GptContext : IGptContext, IDisposable
    {
        private const int DefaultBytesPerSector = 512;
        private const int DefaultChunkSize = 256;
        private readonly uint bytesPerSector;
        private readonly Stream deviceStream;
        private readonly Handler handler;
        private ulong availableSectorSize;
        private ulong currentSector;
        private readonly uint chunkSize;
        private readonly PartitionSegmentCalculator calculator;

        public GptContext(uint diskId, FileAccess fileAccess, uint bytesPerSector = DefaultBytesPerSector, int chunkSize = DefaultChunkSize) : this(
            new DeviceStream(diskId, fileAccess), bytesPerSector)
        {
        }

        public GptContext(string device, FileAccess fileAccess, uint bytesPerSector = DefaultBytesPerSector, uint chunkSize = DefaultChunkSize) : this(
            new DeviceStream(device, fileAccess), bytesPerSector)
        {
        }

        private GptContext(Stream deviceStream, uint bytesPerSector = DefaultBytesPerSector, uint chunkSize = DefaultChunkSize)
        {
            this.deviceStream = deviceStream;
            handler = new Handler(deviceStream, bytesPerSector);
            this.bytesPerSector = bytesPerSector;
            this.chunkSize = chunkSize;

            calculator = new PartitionSegmentCalculator(chunkSize, SizeInSectors);

            var lastSector = handler.Partitions.Select(x => x.LastSector).DefaultIfEmpty().Max();
            var nextSector = calculator.GetNextSector(lastSector);
            currentSector = nextSector;
            availableSectorSize = ToSectors(handler.Length) - lastSector;
        }     

        public ByteSize AvailableSize => new ByteSize(ToBytes(availableSectorSize));

        public ReadOnlyCollection<Partition> Partitions => handler
            .Partitions
            .OrderBy(x => x.FirstSector)
            .ToList().AsReadOnly();

        public ByteSize AllocatedSize => new ByteSize(ToBytes(currentSector));
        public ByteSize TotalSize => new ByteSize(ToBytes(SizeInSectors));

        private ulong SizeInSectors => ToSectors(handler.Length);

        public void Dispose()
        {
            if (deviceStream.CanWrite)
            {
                handler.Commit();
            }

            deviceStream.Dispose();
            Log.Debug("Device Stream disposed");
        }

        public void Add(Entry entry)
        {
            var desiredSize = new GptSegment(currentSector, ToSectors(entry.Size.Bytes));
            var size = calculator.Constraint(desiredSize);

            Log.Debug("PartitionEntry to add: {@Entry}, Desired Size={DesiredSize}, Final Size={FinalSize}", desiredSize, size);

            var partition = new Partition(entry.Name, entry.GptType, bytesPerSector)
            {
                Attributes = entry.Attributes,
                FirstSector = size.Start,
                LastSector = size.End,
                Guid = Guid.NewGuid(),
            };
            
            Log.Debug("Adding pending partition to the context: {@Partition}", partition);
            handler.Partitions.Add(partition);

            EnsureValidLayout(handler.Partitions, SizeInSectors);

            availableSectorSize -= size.Length;
            currentSector += size.Length + chunkSize;
        }

        private static void EnsureValidLayout(IList<Partition> partitions, ulong sizeInSectors)
        {
            var isLayoutValid = PartitionLayoutChecker.IsLayoutValid(partitions, sizeInSectors);
            Log.Debug("Checking [pending] partition layout sanity: Is valid layout? {IsValid}", isLayoutValid);

            if (!isLayoutValid)
            {
                throw new PartitioningException("The desired partition layout is not valid");
            }
        }

        private ulong ToSectors(double sizeInBytes)
        {
            return (ulong)(sizeInBytes / bytesPerSector);
        }

        private ulong ToBytes(double sizeInSectors)
        {
            return (ulong)(sizeInSectors * bytesPerSector);
        }

        public void Delete(Partition partition)
        {
            Log.Debug("Removing partition {Partition}", partition);
            handler.Partitions.Remove(partition);
        }

        public Partition Find(Guid guid)
        {
            Log.Debug("Looking up partition by Guid {Guid}", guid);
            var partition = Partitions.First(x => x.Guid == guid);
            Log.Debug("Obtained partition {Partition}", partition);
            return partition;
        }

        public int IndexOf(Partition partition)
        {
            Log.Debug("Looking up partition index of partition {Partition}", partition);
            var index = Partitions.IndexOf(partition);
            Log.Debug("Partition index={Index}", index);
            return index;
        }
    }
}