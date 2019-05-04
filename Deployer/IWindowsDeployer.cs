using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IWindowsDeployer
    {
        Task Deploy(WindowsDeploymentOptions options, IDevice device, IOperationProgress progressObserver = null);
        Task Backup(Volume windowsVolume, string destination, IOperationProgress progressObserver = null);        
    }
}