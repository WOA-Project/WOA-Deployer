namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class ExecutionFailed : DeployerError
    {
        public Iridio.Common.Errors Errors { get; }

        public ExecutionFailed(Iridio.Common.Errors errors)
        {
            Errors = errors;
        }
    }
}