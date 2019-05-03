using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Injecting drivers")]
    public class InjectDrivers : IDeploymentTask
    {
        private readonly string origin;
        private readonly IDeploymentContext context;
        private readonly IWindowsImageService imageService;

        public InjectDrivers(string origin, IDeploymentContext context, IWindowsImageService imageService)
        {
            this.origin = origin;
            this.context = context;
            this.imageService = imageService;
        }

        public async Task Execute()
        {
            var windowsPartition = await context.Device.GetWindowsVolume();
            await imageService.InjectDrivers(origin, windowsPartition);
        }
    }
}