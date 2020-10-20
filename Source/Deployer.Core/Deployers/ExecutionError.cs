using Iridio.Common;

namespace Deployer.Core.Deployers
{
    public class ExecutionError : DeployError
    {
        public Errors Errors { get; }

        public ExecutionError(Errors errors)
        {
            Errors = errors;
        }
    }
}