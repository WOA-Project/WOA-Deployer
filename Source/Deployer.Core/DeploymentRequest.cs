using Deployer.Core.DeploymentLibrary;

namespace Deployer.Core
{
    public class DeploymentRequest
    {
        public DeviceDto Target { get; }
        public string ScriptPath { get; }

        public DeploymentRequest(DeviceDto target, string scriptPath)
        {
            Target = target;
            ScriptPath = scriptPath;
        }
    }
}