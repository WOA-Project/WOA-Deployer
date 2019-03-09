using System;
using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface IWindowsDeployer
    {
        Task Deploy(WindowsDeploymentOptions options, IDevice device, IObserver<double> progressObserver = null);
        Task Backup(Volume windowsVolume, string destination, IObserver<double> progressObserver = null);        
    }
}