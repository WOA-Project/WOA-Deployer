using System;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IProviderBasedWindowsDeployer
    {
        Task Deploy(IDownloadProgress progressObserver);
        Task Capture(string destination, IDownloadProgress progressObserver);
    }
}