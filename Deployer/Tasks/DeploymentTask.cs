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
        private readonly IFileSystemOperations fileSystemOperations;
        public IOperationContext OperationContext { get; }

        protected DeploymentTask(IDeploymentContext context, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext)
        {
            this.context = context;
            this.fileSystemOperations = fileSystemOperations;
            this.OperationContext = operationContext;
        }

        public Task Execute()
        {
            OperationContext.CancellationToken.ThrowIfCancellationRequested();
            return ExecuteCore();
        }

        protected abstract Task ExecuteCore();

        protected void SaveMetadata(object metadata, string path)
        {
            try
            {
                Log.Debug("Saving metadata {@Metadata}", metadata);
                var dirName = Path.GetDirectoryName(path);
                fileSystemOperations.EnsureDirectoryExists(dirName);
                File.WriteAllText(path, JsonConvert.SerializeObject(metadata, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Error(e, "Cannot save metadata {Metadata} to {Path}", metadata, path);
            }
        }
    }
}