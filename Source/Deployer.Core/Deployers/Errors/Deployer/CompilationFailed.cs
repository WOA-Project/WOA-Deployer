using Deployer.Core.Deployers.Errors.Compiler;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class CompilationFailed : DeployerError
    {
        public DeployerCompilerError Error { get; }

        public CompilationFailed(DeployerCompilerError error)
        {
            Error = error;
        }
    }
}