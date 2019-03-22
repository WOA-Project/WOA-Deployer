using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IDevice
    {
        Task<Disk> GetDeviceDisk();
        Task<Volume> GetWindowsVolume();
        Task<Volume> GetSystemVolume();
        Task<ICollection<DriverMetadata>> GetDrivers();
    }
}