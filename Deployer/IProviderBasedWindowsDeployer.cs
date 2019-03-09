using System;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IProviderBasedWindowsDeployer
    {
        Task Deploy(IObserver<double> progressObserver);
        Task Capture(string destination, IObserver<double> progressObserver);
    }
}