using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Registry;
using Partition = Deployer.FileSystem.Partition;

namespace Deployer
{
    public abstract class Device : IDevice
    {
        protected IDiskApi DiskApi { get; }

        protected Device(IDiskApi diskApi)
        {
            DiskApi = diskApi;
        }

        public abstract Task<Disk> GetDeviceDisk();
        public abstract Task<Volume> GetWindowsVolume();

        protected abstract Task<bool> IsWoAPresent();

        public abstract Task<Volume> GetSystemVolume();

        protected async Task<bool> IsOobeFinished()
        {
            var winVolume = await GetWindowsVolume();

            if (winVolume == null)
            {
                return false;
            }

            var path = Path.Combine(winVolume.Root, "Windows", "System32", "Config", "System");
            if (!File.Exists(path))
            {
                return false;
            }

            var hive = new RegistryHive(path) { RecoverDeleted = true };
            hive.ParseHive();

            var key = hive.GetKey("Setup");
            var val = key.Values.Single(x => x.ValueName == "OOBEInProgress");

            return int.Parse(val.ValueData) == 0;
        }
        
        public async Task<ICollection<DriverMetadata>> GetDrivers()
        {
            var windows = await GetWindowsVolume();
            return await windows.GetDrivers();
        }

        public abstract Task<Partition> GetSystemPartition();
    }
}