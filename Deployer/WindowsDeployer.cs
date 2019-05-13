using System;
using System.Threading;
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

        public async Task Deploy(WindowsDeploymentOptions options, IDevice device,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Information("Applying Windows Image");
            progressObserver?.Percentage.OnNext(double.NaN);
            await imageService.ApplyImage(await device.GetWindowsVolume(), options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver, cancellationToken);
            await MakeBootable(device);
        }

        public Task Backup(Volume windowsVolume, string destination, IOperationProgress progressObserver, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Information("Capturing Windows backup...");
            progressObserver.Percentage.OnNext(double.NaN);
            return imageService.CaptureImage(windowsVolume, destination, progressObserver, cancellationToken);
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