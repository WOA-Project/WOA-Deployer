using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public abstract class DownloaderTask : DeploymentTask
    {
        protected DownloaderTask(IDeploymentContext deploymentContext, IFileSystemOperations fileSystemOperations) : base(deploymentContext, fileSystemOperations)
        {
        }

        protected string ArtifactPath => Path.Combine(AppPaths.ArtifactDownload, ArtifactName);

        protected abstract string ArtifactName { get; }

        protected abstract override Task ExecuteCore();

        protected void SaveMetadata(object metadata)
        {
            SaveMetadata(metadata, Path.Combine(AppPaths.Metadata, ArtifactName, "DownloadInfo.json"));
        }
    }
}