using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public class NullDevice : IDevice
    {
        public Task<Disk> GetDeviceDisk()
        {
            return Task.FromResult(new Disk(null, new DiskInfo()));
        }

        public Task<Volume> GetWindowsVolume()
        {
            return Task.FromResult(new Volume(new Partition(GetNullDisk())));
        }

        public Task<Volume> GetSystemVolume()
        {
            return Task.FromResult(new Volume(new Partition(GetNullDisk())));
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            return Task.FromResult((ICollection<DriverMetadata>)new List<DriverMetadata>());
        }

        private static Disk GetNullDisk()
        {
            return new Disk(new NullDiskApi(), new DiskInfo());
        }
    }
}