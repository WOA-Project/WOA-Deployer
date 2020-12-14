using Iridio.Runtime;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class ExecutionFailed : DeployerError
    {
        public RuntimeErrors Errors { get; }

        public ExecutionFailed(RuntimeErrors errors)
        {
            Errors = errors;
        }
    }
}