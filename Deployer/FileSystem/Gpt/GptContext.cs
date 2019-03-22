using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public class GptContext : IGptContext, IDisposable
    {
        private readonly uint bytesPerSector;
        private readonly Stream deviceStream;
        private readonly Handler handler;
        private ulong availableSectorSize;
        private ulong currentSector;

        public GptContext(uint diskId, FileAccess fileAccess, uint bytesPerSector = 512) : this(
            new DeviceStream(diskId, fileAccess), bytesPerSector)
        {
        }

        public GptContext(string device, FileAccess fileAccess, uint bytesPerSector = 512) : this(
            new DeviceStream(device, fileAccess), bytesPerSector)
        {
        }

        private GptContext(Stream deviceStream, uint bytesPerSector = 512)
        {
            this.deviceStream = deviceStream;
            handler = new Handler(deviceStream, bytesPerSector);
            this.bytesPerSector = bytesPerSector;
            var lastSector = handler.Partitions.Select(x => x.LastSector).DefaultIfEmpty().Max();
            currentSector = lastSector;
            availableSectorSize = ToSectors(handler.Length) - lastSector;
        }

        public ByteSize AvailableSize => new ByteSize(ToBytes(availableSectorSize));

        public IEnumerable<Partition> Partitions => handler.Partitions
            .ToList().AsReadOnly();

        public ByteSize AllocatedSize => new ByteSize(ToBytes(currentSector == 0 ? 0 : currentSector));
        public ByteSize TotalSize => new ByteSize(ToBytes(handler.Length));

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
            var size = ToSectors(entry.Size.Bytes);

            handler.Partitions.Add(new Partition(entry.Name, entry.GptType, (uint)handler.Partitions.Count + 1, bytesPerSector)
            {
                Attributes = entry.Attributes,
                FirstSector = currentSector == 0 ? 0 : currentSector + 1,
                LastSector = Math.Min(currentSector + size, handler.Length),
                PartitionGuid = Guid.NewGuid(),
            });

            availableSectorSize -= size;
            currentSector += size;
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