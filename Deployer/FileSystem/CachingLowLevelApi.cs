using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Zafiro.Core;

namespace Deployer.FileSystem
{
    public class CachingLowLevelApi : ILowLevelApi
    {
        private readonly ILowLevelApi inner;
        private ISet<Disk> disks = new HashSet<Disk>();
        private readonly IDictionary<Disk, List<Partition>> partitions = new Dictionary<Disk, List<Partition>>();
        private readonly IDictionary<Partition, Volume> volumesForPartition = new Dictionary<Partition, Volume>();
        private readonly IDictionary<Disk, IList<Volume>> volumes = new Dictionary<Disk, IList<Volume>>();
        private readonly IDictionary<uint, Disk> diskByNumber = new Dictionary<uint, Disk>();
        
        public CachingLowLevelApi(ILowLevelApi inner)
        {
            this.inner = inner;
        }

        public async Task ResizePartition(Partition partition, ByteSize sizeInBytes)
        {
            await UpdateDisk(partition.Disk);
            await inner.ResizePartition(partition, sizeInBytes);
        }

        private async Task UpdateDisk(Disk disk)
        {
            var newDisk = await GetDisk(disk.Number);

            disks.Remove(disk);
            disks.Add(newDisk);
            diskByNumber[disk.Number] = newDisk;
        }

        public Task<List<Partition>> GetPartitions(Disk disk)
        {
            return partitions.GetCreate(disk, () => GetPartitions(disk));
        }

        public Task<Volume> GetVolume(Partition partition)
        {
            return volumesForPartition.GetCreate(partition, () => GetVolume(partition));
        }

        public async Task<Partition> CreateReservedPartition(Disk disk, ulong sizeInBytes)
        {
            var reservedPartition = await inner.CreateReservedPartition(disk, sizeInBytes);
            partitions[disk].Add(reservedPartition);

            return reservedPartition;
        }

        public async Task<Partition> CreatePartition(Disk disk, ulong sizeInBytes)
        {
            var partition = await inner.CreatePartition(disk, sizeInBytes);
            partitions[disk].Add(partition);

            return partition;
        }

        public Task SetPartitionType(Partition partition, PartitionType partitionType)
        {
            partitions[partition.Disk].First(x => Equals(x, partition)).PartitionType = partitionType;

            return inner.SetPartitionType(partition, partitionType);
        }

        public Task Format(Volume volume, FileSystemFormat ntfs, string fileSystemLabel)
        {
            return inner.Format(volume, ntfs, fileSystemLabel);
        }

        public char GetFreeDriveLetter()
        {
            return inner.GetFreeDriveLetter();
        }

        public Task AssignDriveLetter(Volume volume, char letter)
        {
            return inner.AssignDriveLetter(volume, letter);
        }

        public Task<IList<Volume>> GetVolumes(Disk disk)
        {
            return volumes.GetCreate(disk, () => inner.GetVolumes(disk));
        }

        public Task RemovePartition(Partition partition)
        {
            partitions[partition.Disk].Remove(partition);

            return inner.RemovePartition(partition);
        }

        public async Task<ICollection<Disk>> GetDisks()
        {
            return disks ?? (disks = new HashSet<Disk>(await inner.GetDisks()));
        }

        public Task<Disk> GetDisk(uint diskNumber)
        {
            return inner.GetDisk(diskNumber);
        }

        public Task<ICollection<DriverMetadata>> GetDrivers(string path)
        {
            return inner.GetDrivers(path);
        }

        public Task UpdateStorageCache()
        {
            ClearEverything();

            return inner.UpdateStorageCache();
        }

        public Task ChangeDiskGuid(Disk disk, Guid guid)
        {

            return inner.ChangeDiskGuid(disk, guid);
        }

        private void ClearEverything()
        {
            partitions.Clear();
            volumesForPartition.Clear();
            disks.Clear();
        }
    }
}