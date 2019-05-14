using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Newtonsoft.Json;
using Serilog;

namespace Deployer.Tasks
{
    public abstract class DeploymentTask : IDeploymentTask
    {
        private readonly IDeploymentContext context;

        protected DeploymentTask(IDeploymentContext context)
        {
            this.context = context;
        }

        public Task Execute()
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            return ExecuteCore();
        }

        protected abstract Task ExecuteCore();

        protected static void SaveMetadata(object metadata, string path)
        {
            try
            {
                Log.Debug("Saving metadata {@Metadata}", metadata);
                File.WriteAllText(path, JsonConvert.SerializeObject(metadata, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Error(e, "Cannot save metadata {Metadata} to {Path}", metadata, path);
            }
        }
    }
}