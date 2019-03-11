using System;
using System.Threading.Tasks;
using Serilog;

namespace Deployer
{
    public class ProviderBasedWindowsDeployer : IProviderBasedWindowsDeployer
    {
        private readonly IWindowsOptionsProvider optionsProvider;
        private readonly IDeviceProvider deviceProvider;
        private readonly IWindowsDeployer deployer;

        public ProviderBasedWindowsDeployer(IWindowsOptionsProvider optionsProvider, IDeviceProvider deviceProvider, IWindowsDeployer deployer)
        {
            this.optionsProvider = optionsProvider;
            this.deviceProvider = deviceProvider;
            this.deployer = deployer;
        }

        public async Task Deploy(IDownloadProgress progressObserver)
        {
            Log.Information("Preparing for Windows deployment...");
            var device = deviceProvider.Device;
            var options = optionsProvider.Options;
            await deployer.Deploy(options, device, progressObserver);
        }

        public async Task Capture(string destination, IDownloadProgress progressObserver)
        {
            Log.Information("Preparing for Windows backup...");
            var device = deviceProvider.Device;
            await deployer.Backup(await device.GetWindowsVolume(), destination, progressObserver);
        }       
    }
}