using System;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.FileSystem
{
    public class Partition
    {
        public Partition(Disk disk)
        {
            Disk = disk;
        }

        public Disk Disk { get; }

        public IDiskApi DiskApi => Disk.DiskApi;

        public Guid Guid { get; set; }
        public PartitionType PartitionType { get; set; }
        public uint Number { get; set; }
        public string Name { get; set; }

        public Task Remove()
        {
            return DiskApi.RemovePartition(this);
        }

        public Task Format(FileSystemFormat fileSystemFormat, string label)
        {
            return DiskApi.Format(this, fileSystemFormat, label);
        }

        public Task SetGptType(PartitionType basic)
        {
            return DiskApi.SetGptType(this, basic);
        }

        public Task<Volume> GetVolume()
        {
            return DiskApi.GetVolume(this);
        }

        public Task Resize(ByteSize newSize)
        {
            return DiskApi.ResizePartition(this, newSize);
        }

        public override string ToString()
        {
            return $"Number {Number} in disk {Disk}. {Name ?? "Unnamed"}";
        }
    }
}