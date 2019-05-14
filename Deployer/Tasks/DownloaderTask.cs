using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public abstract class DownloaderTask : DeploymentTask
    {
        protected DownloaderTask(IDeploymentContext deploymentContext) : base(deploymentContext)
        {
        }

        protected abstract string ArtifactPath { get; }

        protected abstract override Task ExecuteCore();

        protected void SaveMetadata(object metadata)
        {
            SaveMetadata(metadata, Path.Combine(ArtifactPath, "DownloadInfo.json"));
        }
    }
}