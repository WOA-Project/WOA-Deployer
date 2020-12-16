using System.Collections.Generic;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public abstract class DeployerError
    {
        public abstract IEnumerable<string> Items { get; }
    }
}