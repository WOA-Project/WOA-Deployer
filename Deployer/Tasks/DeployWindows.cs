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
        private readonly IDeploymentContext deploymentContext;
        private readonly IWindowsDeployer deployer;
        private readonly IOperationProgress progressObserver;

        public DeployWindows(IDeploymentContext deploymentContext, IWindowsDeployer deployer, IOperationProgress progressObserver,
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(deploymentContext, fileSystemOperations, operationContext)
        {
            this.deploymentContext = deploymentContext;
            this.deployer = deployer;
            this.progressObserver = progressObserver;
        }

        protected override async Task ExecuteCore()
        {
            Log.Information("Deploying Windows...");
            await deploymentContext.DiskLayoutPreparer.Prepare(await deploymentContext.Device.GetDeviceDisk());
            await deployer.Deploy(deploymentContext.DeploymentOptions, deploymentContext.Device, progressObserver, OperationContext.CancellationToken);
        }
    }
}