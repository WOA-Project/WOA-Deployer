using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    public abstract class DeploymentTask : IDeploymentTask
    {
        private readonly IDeploymentContext context;

        protected DeploymentTask(IDeploymentContext context)
        {
            this.context = context;
        }

        public Task Execute()
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            return ExecuteCore();
        }

        protected abstract Task ExecuteCore();
    }
}