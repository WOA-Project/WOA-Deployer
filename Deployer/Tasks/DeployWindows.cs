using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows : IDeploymentTask
    {
        private readonly IDeploymentContext context;
        private readonly IWindowsDeployer deployer;
        private readonly IOperationProgress progressObserver;

        public DeployWindows(IDeploymentContext context, IWindowsDeployer deployer, IOperationProgress progressObserver)
        {
            this.context = context;
            this.deployer = deployer;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            Log.Information("Deploying Windows...");
            await context.DiskLayoutPreparer.Prepare(await context.Device.GetDeviceDisk());
            await deployer.Deploy(context.DeploymentOptions, context.Device, progressObserver);
        }
    }
}