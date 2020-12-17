using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
{
    public class DeviceDeployer : IDeviceDeployer
    {
        private const string FeedFolder = "Feed";
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");

        private readonly IWoaDeployer deployer;
        private readonly IFileSystemOperations fileSystemOperations;

        public DeviceDeployer(IWoaDeployer deployer, IFileSystemOperations fileSystemOperations)
        {
            this.deployer = deployer;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task<Either<DeployerError, Success>> Deploy(DeploymentRequest deploymentRequest)
        {
            await DownloadFeed();
            return await Run(Path.Combine(FeedFolder, deploymentRequest.ScriptPath));
        }

        private async Task DownloadFeed()
        {
            await DeleteFeedFolder();
            await Run(BootstrapPath);
        }

        private Task<Either<DeployerError, Success>> Run(string bootstrapPath)
        {
            return deployer.Run(bootstrapPath);
        }

        private async Task DeleteFeedFolder()
        {
            if (fileSystemOperations.DirectoryExists(FeedFolder))
            {
                await fileSystemOperations.DeleteDirectory(FeedFolder);
            }
        }
    }
}