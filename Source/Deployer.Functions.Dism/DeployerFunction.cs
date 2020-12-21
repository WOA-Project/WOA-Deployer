using System;
using System.IO;
using Deployer.Core.Scripting;
using Newtonsoft.Json;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions
{
    public abstract class DeployerFunction : IDeployerFunction
    {
        protected DeployerFunction(IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext)
        {
            FileSystemOperations = fileSystemOperations;
            OperationContext = operationContext;
        }

        protected IFileSystemOperations FileSystemOperations { get; }
        protected IOperationContext OperationContext { get; }

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