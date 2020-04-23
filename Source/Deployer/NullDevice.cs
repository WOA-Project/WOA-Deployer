using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public class NullDevice : IDevice
    {
        public Task<IDisk> GetDeviceDisk()
        {
            return Task.FromResult((IDisk)new TestDisk());
        }

        public Task<IPartition> GetWindowsPartition()
        {
            throw new System.NotImplementedException();
        }

        public Task<IPartition> GetSystemPartition()
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            return Task.FromResult((ICollection<DriverMetadata>)new List<DriverMetadata>());
        }
    }
}