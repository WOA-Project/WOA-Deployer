using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.FileSystem.Gpt;
using Registry;
using Serilog;
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

    public static class GptContextFactory
    {
        public static Task<GptContext> Create(uint diskId, FileAccess fileAccess,
            uint bytesPerSector = GptContext.DefaultBytesPerSector, uint chuckSize = GptContext.DefaultChunkSize)
        {
            return Observable
                .Defer(() => Observable.Return(new GptContext(diskId, fileAccess, bytesPerSector, chuckSize)))
                .RetryWithBackoffStrategy()
                .ToTask();
        }
    }
}