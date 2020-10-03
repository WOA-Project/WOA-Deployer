using System.Collections.Generic;
using Iridio.Binding.Model;
using Iridio.Common;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Compiler
{
    public interface IDeployerCompiler
    {
        Either<Errors, CompilationUnit> Compile(string path, IEnumerable<Assignment> toInject);
    }
}