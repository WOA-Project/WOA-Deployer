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
        public Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken))
        {
            Log.Verbose("Applying Windows Image {Image}{Index} to {Volume}", imagePath, imageIndex, volume.Label);
            return Task.CompletedTask;
        }

        public Task<IList<string>> InjectDrivers(string path, Volume volume)
        {
            Log.Verbose("Injecting drivers from {Path} into {Volume}", path, volume.Label);

            return Task.FromResult((IList<string>)new List<string>());
        }

        public Task RemoveDriver(string path, Volume volume)
        {
            Log.Verbose("Removing driver {Path} from {Volume}", path, volume);
            
            return Task.CompletedTask;
        }

        public Task CaptureImage(Volume windowsVolume, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Verbose("Capturing image");

            return Task.CompletedTask;
        }
    }
}