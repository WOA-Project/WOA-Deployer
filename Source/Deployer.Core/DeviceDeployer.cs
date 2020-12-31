using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
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

        public async Task<Either<DeployerError, Success>> Deploy(DeploymentRequest deploymentRequest)
        {
            return await Run(Path.Combine(FeedFolder, deploymentRequest.ScriptPath));
        }

        private Task<Either<DeployerError, Success>> Run(string bootstrapPath)
        {
            return deployer.Run(bootstrapPath);
        }
    }
}