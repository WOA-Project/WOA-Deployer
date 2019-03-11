using System;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IWindowsDeployer
    {
        Task Deploy(WindowsDeploymentOptions options, IDevice device, IDownloadProgress progressObserver = null);
        Task Backup(Volume windowsVolume, string destination, IDownloadProgress progressObserver = null);        
    }
}