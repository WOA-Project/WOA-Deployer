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
        protected DeploymentTask(IDeploymentContext deploymentContext, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext)
        {
            DeploymentContext = deploymentContext;
            FileSystemOperations = fileSystemOperations;
            OperationContext = operationContext;
        }

        protected IDeploymentContext DeploymentContext { get; }
        protected IFileSystemOperations FileSystemOperations { get; }
        protected IOperationContext OperationContext { get; }

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
                FileSystemOperations.EnsureDirectoryExists(dirName);
                File.WriteAllText(path, JsonConvert.SerializeObject(metadata, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Error(e, "Cannot save metadata {Metadata} to {Path}", metadata, path);
            }
        }
    }
}