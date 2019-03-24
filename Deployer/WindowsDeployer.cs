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
            progressObserver.Percentage.OnNext(double.NaN);
            await imageService.ApplyImage(await device.GetWindowsVolume(), options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver);
            await MakeBootable(device);
        }

        public Task Backup(Volume windowsVolume, string destination, IDownloadProgress progressObserver)
        {
            Log.Information("Capturing Windows backup...");
            progressObserver.Percentage.OnNext(double.NaN);
            return imageService.CaptureImage(windowsVolume, destination, progressObserver);
        }

        private async Task MakeBootable(IDevice device)
        {
            Log.Verbose("Making Windows installation bootable...");

            var windowsVolume = await device.GetWindowsVolume();
            var systemVolume = await device.GetSystemVolume();

            await bootCreator.MakeBootable(systemVolume, windowsVolume);
        }       
    }
}