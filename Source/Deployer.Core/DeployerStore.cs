using System.Collections.Generic;

namespace Deployer.Core
{
    public class DeployerStore 
    {
        public IEnumerable<Device> Devices { get; set; }
        public IEnumerable<Deployment> Deployments { get; set; }
    }
}