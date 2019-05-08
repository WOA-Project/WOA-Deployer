using Deployer.Tasks;

namespace Deployer.Tests
{
    public class TestDeploymentContext : IDeploymentContext
    {
        public IDiskLayoutPreparer DiskLayoutPreparer { get; set; }
        public IDevice Device { get; set; }
        public WindowsDeploymentOptions DeploymentOptions { get; set; }
    }
}