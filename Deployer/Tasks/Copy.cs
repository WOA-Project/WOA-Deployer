using System.Threading;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Copying file {0} to {1}")]
    public class Copy : DeploymentTask
    {
        private readonly string origin;
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;

        public Copy(string origin, string destination, IFileSystemOperations fileSystemOperations, IDeploymentContext deploymentContext) : base(deploymentContext)
        {
            this.origin = origin;
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
        }

        protected override Task ExecuteCore()
        {
            return fileSystemOperations.Copy(origin, destination);
        }
    }
}