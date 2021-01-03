using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Iridio.Binding.Model;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Ide
{
    public interface IIdeDeployerCompiler
    {
        Task<Either<DeployerCompilerError, Script>> Compile(string path);
    }
}