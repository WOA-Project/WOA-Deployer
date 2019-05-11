using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Newtonsoft.Json;
using Serilog;

namespace Deployer.Tasks
{
    public abstract class DownloaderTask : IDeploymentTask
    {
        public abstract Task Execute();

        public void SaveMetadata(object metadata)
        {
            Log.Debug("Saving metadata {@Metadata}", metadata);
            File.WriteAllText(Path.Combine(ArtifactPath, "Info.json"), JsonConvert.SerializeObject(metadata, Formatting.Indented));
        }

        public abstract string ArtifactPath { get; }
    }
}