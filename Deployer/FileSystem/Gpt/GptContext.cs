using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;

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

        public IEnumerable<Partition> Partitions => handler.Partitions
            .ToList().AsReadOnly();

        public ByteSize AllocatedSize => new ByteSize(ToBytes(currentSector == 0 ? 0 : currentSector));
        public ByteSize TotalSize => new ByteSize(ToBytes(SizeInSectors));

        private ulong SizeInSectors => ToSectors(handler.Length);

        public void Dispose()
        {
            if (deviceStream.CanWrite)
            {
                handler.Commit();
            }

            deviceStream.Dispose();
        }

        public void Add(Entry entry)
        {
            var desiredSize = new GptSegment(currentSector, ToSectors(entry.Size.Bytes));
            var size = calculator.Constraint(desiredSize);

            var partition = new Partition(entry.Name, entry.GptType, (uint)handler.Partitions.Count + 1, bytesPerSector)
            {
                Attributes = entry.Attributes,
                FirstSector = size.Start,
                LastSector = size.End,
                PartitionGuid = Guid.NewGuid(),
            };

            handler.Partitions.Add(partition);

            availableSectorSize -= size.Length;
            currentSector += size.Length + chunkSize;
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
            handler.Partitions.Remove(partition);
        }
    }
}