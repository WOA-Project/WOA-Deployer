using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Core.FileSystem;

namespace Deployer.NetFx.Tests
{
    public class TestDevice
    {
        public Task<IDisk> GetDeviceDisk()
        {
            throw new System.NotImplementedException();
        }

        public Task<IPartition> GetWindowsPartition()
        {
            throw new System.NotImplementedException();
        }

        public Task<IPartition> GetSystemPartition()
        {
            throw new System.NotImplementedException();
        }

        public Task<IVolume> GetSystemVolume()
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            throw new System.NotImplementedException();
        }
    }
}