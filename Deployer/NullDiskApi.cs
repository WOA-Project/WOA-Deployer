using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;

namespace Deployer
{
    internal class NullDiskApi : IDiskApi
    {
        public Task<List<Disk>> GetDisks()
        {
            throw new NotImplementedException();
        }

        public Task<List<Partition>> GetPartitions(Disk disk)
        {
            throw new NotImplementedException();
        }

        public Task Format(Partition partition, FileSystemFormat fileSystemFormat, string label = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Volume>> GetVolumes(Disk disk)
        {
            throw new NotImplementedException();
        }

        public Task RemovePartition(Partition partition)
        {
            throw new NotImplementedException();
        }

        public Task RefreshDisk(Disk disk)
        {
            throw new NotImplementedException();
        }

        public char GetFreeDriveLetter()
        {
            throw new NotImplementedException();
        }

        public Task AssignDriveLetter(Partition partition, char driveLetter)
        {
            throw new NotImplementedException();
        }

        public Task SetGptType(Partition partition, PartitionType partitionType)
        {
            throw new NotImplementedException();
        }

        public Task<Volume> GetVolume(Partition partition)
        {
            throw new NotImplementedException();
        }

        public Task ResizePartition(Partition partition, ByteSize size)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<DriverMetadata>> GetDrivers(Volume volume)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDiskId(Disk disk, Guid guid)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStorageCache()
        {
            throw new NotImplementedException();
        }

        public Task<Disk> GetDisk(int n)
        {
            throw new NotImplementedException();
        }
    }
}