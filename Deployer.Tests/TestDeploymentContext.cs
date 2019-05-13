using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Deployer.Tasks;

namespace Deployer.Tests
{
    public class TestDeploymentContext : IDeploymentContext
    {
        public IDiskLayoutPreparer DiskLayoutPreparer { get; set; }
        public IDevice Device { get; set; }
        public WindowsDeploymentOptions DeploymentOptions { get; set; }
        public CancellationToken CancellationToken { get; }
        public ISubject<Unit> Cancelled { get; }

        public void Cancel()
        {
        }

        public void Start()
        {
        }
    }
}