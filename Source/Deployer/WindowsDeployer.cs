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

        public WindowsDeployer(IWindowsImageService imageService)
        {
            this.imageService = imageService;
        }

        public async Task Deploy(WindowsDeploymentOptions options, IDevice device,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default)
        {
            Log.Information("Applying Windows Image");
            progressObserver?.Percentage.OnNext(double.NaN);
            var partition = await device.GetWindowsPartition();
            
            await imageService.ApplyImage(partition, options.ImagePath, options.ImageIndex, options.UseCompact, progressObserver, cancellationToken);
        }

        public Task Backup(IPartition partition, string destination, IOperationProgress progressObserver, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Information("Capturing Windows backup...");
            progressObserver.Percentage.OnNext(double.NaN);
            return imageService.CaptureImage(partition, destination, progressObserver, cancellationToken);
        }
    }
}