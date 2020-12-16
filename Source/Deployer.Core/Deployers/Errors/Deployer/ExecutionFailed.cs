using System.Collections.Generic;
using System.Linq;
using Iridio.Runtime;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class ExecutionFailed : DeployerError
    {
        public RuntimeErrors Errors { get; }

        public ExecutionFailed(RuntimeErrors errors)
        {
            Errors = errors;
        }

        public override IEnumerable<string> Items => Errors.SelectMany(x => x.Items);
    }
}