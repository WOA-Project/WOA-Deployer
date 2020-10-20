namespace Deployer.Core.Deployers
{
    class RequirementsError : DeployError
    {
        public Requirements.Error Error { get; }

        public RequirementsError(Requirements.Error error)
        {
            Error = error;
        }
    }
}