using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
{
    public class DeviceDeployer : IDeviceDeployer
    {
        private const string FeedFolder = "Feed";

        private readonly IWoaDeployer deployer;

        public DeviceDeployer(IWoaDeployer deployer)
        {
            this.deployer = deployer;
        }

        public async Task<Either<DeployerError, DeploymentSuccess>> Deploy(DeploymentRequest deploymentRequest)
        {
            return await Run(Path.Combine(FeedFolder, deploymentRequest.ScriptPath));
        }

        private Task<Either<DeployerError, DeploymentSuccess>> Run(string bootstrapPath)
        {
            return deployer.Run(bootstrapPath);
        }
    }

    public class DeploymentSuccess
    {

    }
}