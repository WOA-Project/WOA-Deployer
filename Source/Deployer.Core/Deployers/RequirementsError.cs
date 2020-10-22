using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Deployers
{
    class RequirementsError : DeployError
    {
        public ErrorList Errors { get; }

        public RequirementsError(ErrorList errors)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return string.Join(", ", Errors);
        }
    }
}