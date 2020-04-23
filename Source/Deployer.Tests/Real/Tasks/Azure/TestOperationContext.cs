using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Deployer.Core.Scripting;

namespace Deployer.Tests.Real.Tasks.Azure
{
    public class TestOperationContext : IOperationContext
    {
        public CancellationToken CancellationToken { get; }
        IObservable<Unit> IOperationContext.Cancelled => Cancelled;

        public ISubject<Unit> Cancelled { get; }
        public IObservable<Unit> Cancelling { get; }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void SetOperationAsCancelled()
        {
            throw new NotImplementedException();
        }
    }
}