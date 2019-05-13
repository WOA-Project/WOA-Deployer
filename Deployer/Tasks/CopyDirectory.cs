using System.Threading;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Copying folder {0} to {1}")]
    public class CopyDirectory : DeploymentTask
    {
        private readonly string origin;
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;

        public CopyDirectory(string origin, string destination, IFileSystemOperations fileSystemOperations, IDeploymentContext deploymentContext) : base(deploymentContext)
        {
            this.origin = origin;
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
        }

        protected override Task ExecuteCore()
        {
            return fileSystemOperations.CopyDirectory(origin, destination);
        }
    }
}