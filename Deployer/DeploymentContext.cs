using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Deployer.Tasks;

namespace Deployer
{
    public class DeploymentContext: IDeploymentContext
    {
        private CancellationTokenSource cancellationTokenSource;
        public IDiskLayoutPreparer DiskLayoutPreparer { get; set; } = new NullDiskPreparer();
        public IDevice Device { get; set; } = new NullDevice();
        public WindowsDeploymentOptions DeploymentOptions { get; set; } = new WindowsDeploymentOptions();
        public ISubject<Unit> Cancelled { get; } = new Subject<Unit>();

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;
    }
}