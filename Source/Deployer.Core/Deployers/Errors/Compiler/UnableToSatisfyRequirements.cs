using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Deployers.Errors.Compiler
{
    public class UnableToSatisfyRequirements : DeployerCompilerError
    {
        public ErrorList Errors { get; }

        public UnableToSatisfyRequirements(ErrorList errors)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return string.Join(", ", Errors);
        }
    }
}