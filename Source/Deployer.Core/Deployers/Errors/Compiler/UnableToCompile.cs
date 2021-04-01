using System.Collections.Generic;
using Iridio;

namespace Deployer.Core.Deployers.Errors.Compiler
{
    public class UnableToCompile : DeployerCompilerError
    {
        public CompilerError Error { get; }

        public UnableToCompile(CompilerError error)
        {
            Error = error;
        }

        public override string ToString()
        {
            return string.Join(", ", Error);
        }

        public override IEnumerable<string> Items => new[] { Error.ToString() };
    }
}