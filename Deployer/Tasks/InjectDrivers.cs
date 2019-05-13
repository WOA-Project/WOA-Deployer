using System.Threading;
using System.Threading.Tasks;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Injecting drivers")]
    public class InjectDrivers : DeploymentTask
    {
        private readonly string origin;
        private readonly IDeploymentContext context;
        private readonly IWindowsImageService imageService;

        public InjectDrivers(string origin, IDeploymentContext context, IWindowsImageService imageService,
            IDeploymentContext deploymentContext) : base(deploymentContext)
        {
            this.origin = origin;
            this.context = context;
            this.imageService = imageService;
        }

        protected override async Task ExecuteCore()
        {
            var windowsPartition = await context.Device.GetWindowsVolume();
            await imageService.InjectDrivers(origin, windowsPartition);
        }
    }
}