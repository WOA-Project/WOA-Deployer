using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Registry;
using Serilog;

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

        protected async Task<Volume> GetVolumeByPartitionName(string partitionName, bool automount = true)
        {
            Log.Verbose("Getting volume by partition name '{Name}'", partitionName);

            var disk = await GetDeviceDisk();
            var vol = await disk.GetVolumeByPartitionName(partitionName);

            if (automount && vol != null)
            {
                if (vol.Root == null)
                {
                    await vol.Mount();
                }
            }

            return vol;
        }

        protected async Task<Volume> GetVolumeByLabel(string label, bool automount = true)
        {
            Log.Verbose("Getting volume labeled as {Label}", label);

            var disk = await GetDeviceDisk();
            var vol = await disk.GetVolumeByLabel(label);

            if (automount && vol != null)
            {
                if (vol.Root == null)
                {
                    await vol.Mount();
                }
            }

            return vol;
        }

        protected async Task<bool> IsWoAPresent()
        {
            try
            {
                await IsBootVolumePresent();
                await GetWindowsVolume();
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to get WoA's volumes");
                return false;
            }

            return true;
        }

        private async Task<bool> IsBootVolumePresent()
        {
            var bootPartition = await GetSystemPartition();

            if (bootPartition != null)
            {
                return true;
            }

            var bootVolume = await GetSystemVolume();
            return bootVolume != null;
        }

        public abstract Task<Volume> GetSystemVolume();

        protected async Task<bool> IsOobeFinished()
        {
            var winVolume = await GetWindowsVolume();

            if (winVolume == null)
            {
                return false;
            }

            var path = Path.Combine(winVolume.Root, "Windows", "System32", "Config", "System");
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