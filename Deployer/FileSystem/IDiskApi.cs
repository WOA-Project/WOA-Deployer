using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.FileSystem
{
    public interface IDiskApi
    {
        Task<List<Disk>> GetDisks();
        Task<List<Partition>> GetPartitions(Disk disk);
        Task Format(Partition partition, FileSystemFormat fileSystemFormat, string label = null);
        Task<IList<Volume>> GetVolumes(Disk disk);
        Task RemovePartition(Partition partition);
        Task RefreshDisk(Disk disk);
        char GetFreeDriveLetter();
        Task AssignDriveLetter(Volume volume, char driverLetter);
        Task SetGptType(Partition partition, PartitionType partitionType);
        Task<Volume> GetVolume(Partition partition);
        Task ResizePartition(Partition partition, ByteSize size);
        Task<ICollection<DriverMetadata>> GetDrivers(Volume volume);
        Task ChangeDiskId(Disk disk, Guid guid);
        Task UpdateStorageCache();
        Task<Disk> GetDisk(int i);
        Task SetGuid(Disk disk, Guid guid);
    }
}