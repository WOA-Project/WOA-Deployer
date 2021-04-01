using System.Collections.Generic;
using System.Linq;
using Iridio.Runtime;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class ExecutionFailed : DeployerError
    {
        public RunError Error { get; }

        public ExecutionFailed(RunError error)
        {
            Error = error;
        }

        public override IEnumerable<string> Items => new []{ Error.ToString() };

        public override string ToString()
        {
            return string.Join(";", Items.Select(s => s));
        }
    }
}