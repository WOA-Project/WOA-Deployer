using System.Collections.Generic;

namespace Deployer.Core.Deployers.Errors.Compiler
{
    public abstract class DeployerCompilerError
    {
        public abstract IEnumerable<string> Items { get; }
    }
}