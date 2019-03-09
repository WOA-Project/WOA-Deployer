using System;
using System.Threading.Tasks;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestProviderBasedWindowsDeployer : IProviderBasedWindowsDeployer
    {
        public Task Deploy(IObserver<double> progressObserver)
        {
            Log.Verbose("Deploying Windows");
            return Task.CompletedTask;
        }

        public Task Capture(string destination, IObserver<double> progressObserver)
        {
            Log.Verbose("Backing up Windows");
            return Task.CompletedTask;
        }
    }
}