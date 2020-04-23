using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IDevice
    {
        Task<IDisk> GetDeviceDisk();
        Task<IPartition> GetWindowsPartition();
        Task<IPartition> GetSystemPartition();
        Task<ICollection<DriverMetadata>> GetDrivers();
    }
}