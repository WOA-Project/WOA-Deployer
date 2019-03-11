using System;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer
{
    public class WindowsDeployer : IWindowsDeployer
    {
        private readonly IWindowsImageService imageService;
        private readonly IBootCreator bootCreator;

        public WindowsDeployer(IWindowsImageService imageService, IBootCreator bootCreator)
        {
            this.imageService = imageService;
            this.bootCreator = bootCreator;
        }

        public async Task Deploy(WindowsDeploymentOptions options, IDevice device, IDownloadProgress progressObserver)
        {
            Log.Information("Deploying Windows...");
            await imageService.ApplyImage(await device.GetWindowsVolume(), options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver);
            await MakeBootable(device);
        }

        public Task Backup(Volume windowsVolume, string destination, IDownloadProgress progressObserver)
        {
            Log.Information("Capturing Windows backup...");
            return imageService.CaptureImage(windowsVolume, destination, progressObserver);
        }

        private async Task MakeBootable(IDevice device)
        {
            Log.Verbose("Making Windows installation bootable...");

            var boot = await device.GetBootVolume();
            var windows = await device.GetWindowsVolume();

            await bootCreator.MakeBootable(boot, windows);
            await boot.Partition.SetGptType(PartitionType.Esp);
            var updatedBootVolume = await device.GetBootVolume();

            if (updatedBootVolume != null)
            {
                Log.Verbose("We shouldn't be able to get a reference to the Boot volume.");
                Log.Verbose("Updated Boot Volume: {@Volume}", new { updatedBootVolume.Label, updatedBootVolume.Partition, });
                if (!Equals(updatedBootVolume.Partition.PartitionType, PartitionType.Esp))
                {
                    Log.Warning("The system partition should be {Esp}, but it's {ActualType}", PartitionType.Esp, updatedBootVolume.Partition.PartitionType);
                }
            }
        }
    }
}