using System.Collections.Generic;

namespace Deployer.Core.DeploymentLibrary
{
    public class DeployerStore 
    {
        public IEnumerable<Device> Devices { get; set; }
        public IEnumerable<Deployment> Deployments { get; set; }
    }
}