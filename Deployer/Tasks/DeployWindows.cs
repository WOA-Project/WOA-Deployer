using System;
using System.Threading.Tasks;
using Deployer.Execution;

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
            await preparer.Prepare(await deviceProvider.Device.GetDeviceDisk());
            await providerBasedWindowsDeployer.Deploy(progressObserver);
        }
    }
}