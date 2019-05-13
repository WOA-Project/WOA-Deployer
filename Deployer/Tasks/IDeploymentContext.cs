using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;

namespace Deployer.Tasks
{
    public interface IDeploymentContext
    {
        IDiskLayoutPreparer DiskLayoutPreparer { get; set; }
        IDevice Device { get; set; }
        WindowsDeploymentOptions DeploymentOptions { get; set; }
        CancellationToken CancellationToken { get; }
        ISubject<Unit> Cancelled { get; }
        void Cancel();
        void Start();
    }
}