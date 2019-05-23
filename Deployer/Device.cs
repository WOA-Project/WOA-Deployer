using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Registry;

namespace Deployer
{
    public abstract class Device : IDevice
    {
        public abstract Task<IDisk> GetDeviceDisk();
        public abstract Task<IPartition> GetWindowsPartition();

        protected abstract Task<bool> IsWoAPresent();

        public abstract Task<IPartition> GetSystemPartition();

        protected async Task<bool> IsOobeFinished()
        {
            var winVolume = await GetWindowsPartition();

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
            var winPart = await GetWindowsPartition();
            var vol = await winPart.GetVolume();
            return await vol.GetDrivers();
        }
    }
}