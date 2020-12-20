using System.Collections.Generic;
using System.Linq;
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

        public override IEnumerable<string> Items => Error.Items.Select(s => s);

        public override string ToString()
        {
            return string.Join(";", Items.Select(s => s));
        }
    }
}