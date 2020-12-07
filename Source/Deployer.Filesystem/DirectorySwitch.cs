using System;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Filesystem
{
    public class DirectorySwitch : IDisposable
    {
        private readonly IFileSystemOperations operations;
        private readonly string oldDirectory;

        public DirectorySwitch(IFileSystemOperations operations, string directory)
        {
            this.operations = operations;
            Log.Debug($"Switching to {directory}");

            oldDirectory = operations.WorkingDirectory;
            operations.WorkingDirectory = directory;
        }

        public void Dispose()
        {
            Log.Debug($"Returning to {oldDirectory}");
            operations.WorkingDirectory = oldDirectory;
        }
    }
}