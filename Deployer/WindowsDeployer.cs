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
            var partition = await device.GetWindowsPartition();
            
            await imageService.ApplyImage(partition, options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver, cancellationToken);
            await MakeBootable(device);
        }

        public Task Backup(IPartition partition, string destination, IOperationProgress progressObserver, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Information("Capturing Windows backup...");
            progressObserver.Percentage.OnNext(double.NaN);
            return imageService.CaptureImage(partition, destination, progressObserver, cancellationToken);
        }

        private async Task MakeBootable(IDevice device)
        {
            Log.Verbose("Making Windows installation bootable...");

            var windows = await device.GetWindowsPartition();
            var system = await device.GetSystemPartition();

            await bootCreator.MakeBootable(system, windows);
            await system.SetGptType(PartitionType.Esp);
        }       
    }
}