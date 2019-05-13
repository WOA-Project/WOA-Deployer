using System.Threading;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Services
{
    public interface IWindowsImageService
    {
        Task ApplyImage(Volume windowsVolume, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken));
        Task InjectDrivers(string path, Volume windowsPartition);
        Task RemoveDriver(string path, Volume volume);
        Task CaptureImage(Volume windowsVolume, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}