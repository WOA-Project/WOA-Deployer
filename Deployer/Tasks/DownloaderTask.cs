using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Deployer.Tasks
{
    public abstract class DownloaderTask : DeploymentTask
    {
        protected DownloaderTask(IDeploymentContext deploymentContext) : base(deploymentContext)
        {
        }

        public abstract string ArtifactPath { get; }
        protected abstract override Task ExecuteCore();

        public void SaveMetadata(object metadata)
        {
            Log.Debug("Saving metadata {@Metadata}", metadata);
            File.WriteAllText(Path.Combine(ArtifactPath, "Info.json"),
                JsonConvert.SerializeObject(metadata, Formatting.Indented));
        }
    }
}