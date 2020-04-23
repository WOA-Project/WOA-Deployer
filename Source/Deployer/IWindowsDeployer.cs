using System.Threading;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IWindowsDeployer
    {
        Task Deploy(WindowsDeploymentOptions options, IDevice device, IOperationProgress progressObserver = null,
            CancellationToken cancellationToken = default(CancellationToken));
        Task Backup(IPartition partition, string destination, IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken));        
    }
}