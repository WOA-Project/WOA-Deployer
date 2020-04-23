using System.Threading.Tasks;

namespace Deployer.Tasks
{
    [TaskDescription("Making Installation bootable")]
    [Requires(Dependency.DeploymentOptions)]
    public class MakeWindowsBootable : DeploymentTask
    {
        private readonly IDeploymentContext deploymentContext;
        private readonly IBootCreator bootCreator;

        public MakeWindowsBootable(IDeploymentContext deploymentContext, IBootCreator bootCreator,
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(deploymentContext, fileSystemOperations, operationContext)
        {
            this.deploymentContext = deploymentContext;
            this.bootCreator = bootCreator;
        }

        protected override async Task ExecuteCore()
        {
            await bootCreator.MakeBootable(await deploymentContext.Device.GetSystemPartition(),
                await deploymentContext.Device.GetWindowsPartition());
        }
    }
}