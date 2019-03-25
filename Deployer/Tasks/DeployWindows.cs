using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Deploying Windows")]
    public class DeployWindows : IDeploymentTask
    {
        private readonly IDeviceProvider deviceProvider;
        private readonly IDiskLayoutPreparer preparer;
        private readonly IProviderBasedWindowsDeployer providerBasedWindowsDeployer;
        private readonly IDownloadProgress progressObserver;

        public DeployWindows(IDeviceProvider deviceProvider, IDiskLayoutPreparer preparer, IProviderBasedWindowsDeployer providerBasedWindowsDeployer, IDownloadProgress progressObserver)
        {
            this.deviceProvider = deviceProvider;
            this.preparer = preparer;
            this.providerBasedWindowsDeployer = providerBasedWindowsDeployer;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            Log.Information("Preparing disk layout...");
            await preparer.Prepare(await deviceProvider.Device.GetDeviceDisk());

            Log.Information("Applying Windows image...");
            await providerBasedWindowsDeployer.Deploy(progressObserver);
        }
    }
}