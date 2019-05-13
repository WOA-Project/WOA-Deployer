using System.Threading;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    [Requires(Dependency.DeploymentOptions)]
    public class DeployWindows : DeploymentTask
    {
        private readonly IDeploymentContext context;
        private readonly IWindowsDeployer deployer;
        private readonly IOperationProgress progressObserver;

        public DeployWindows(IDeploymentContext context, IWindowsDeployer deployer, IOperationProgress progressObserver) : base(context)
        {
            this.context = context;
            this.deployer = deployer;
            this.progressObserver = progressObserver;
        }

        protected override async Task ExecuteCore()
        {
            Log.Information("Deploying Windows...");
            await context.DiskLayoutPreparer.Prepare(await context.Device.GetDeviceDisk());
            await deployer.Deploy(context.DeploymentOptions, context.Device, progressObserver, context.CancellationToken);
        }
    }
}