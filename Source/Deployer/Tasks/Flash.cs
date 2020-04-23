using System.Threading.Tasks;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Flashing image {0}")]
    public class Flash : DeploymentTask
    {
        private readonly string imagePath;
        private readonly IImageFlasher flasher;
        private readonly IOperationProgress progress;

        public Flash(string imagePath, IImageFlasher flasher, IDeploymentContext deploymentContext,
            IOperationContext operationContext, IFileSystemOperations fileSystemOperations, IOperationProgress progress) : base(deploymentContext,
            fileSystemOperations, operationContext)
        {
            this.imagePath = imagePath;
            this.flasher = flasher;
            this.progress = progress;
        }

        protected override async Task ExecuteCore()
        {
            var deviceDisk = await DeploymentContext.Device.GetDeviceDisk();
            await flasher.Flash(deviceDisk, imagePath, progress);
        }
    }
}