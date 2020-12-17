namespace Deployer.Core
{
    public class DeploymentRequest
    {
        public Device Target { get; }
        public string ScriptPath { get; }

        public DeploymentRequest(Device target, string scriptPath)
        {
            Target = target;
            ScriptPath = scriptPath;
        }
    }
}