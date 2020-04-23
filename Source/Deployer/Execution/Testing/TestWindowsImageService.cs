using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestWindowsImageService : IWindowsImageService
    {
        public Task ApplyImage(IPartition applyPath, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken))
        {
            Log.Verbose("Applying Windows Image {Image}{Index} to {Volume}", imagePath, imageIndex, applyPath);
            return Task.CompletedTask;
        }

        public Task<IList<string>> InjectDrivers(string path, string windowsRootPath)
        {
            Log.Verbose("Injecting drivers from {Path} into {Volume}", path, windowsRootPath);

            return Task.FromResult((IList<string>)new List<string>());
        }

        public Task RemoveDriver(string path, string windowsRootPath)
        {
            Log.Verbose("Removing driver {Path} from {Volume}", path, windowsRootPath);
            
            return Task.CompletedTask;
        }

        public Task CaptureImage(IPartition windowsVolume, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Verbose("Capturing image");

            return Task.CompletedTask;
        }
    }
}