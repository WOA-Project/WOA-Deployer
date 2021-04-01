using System.Collections.Generic;
using Iridio;
using Iridio.Binding.Model;
using Iridio.Common;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Compiler
{
    public interface IDeployerCompiler
    {
        Either<CompilerError, Script> Compile(string path, IEnumerable<Assignment> toInject);
    }
}