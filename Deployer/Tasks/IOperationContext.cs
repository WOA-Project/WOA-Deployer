using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;

namespace Deployer.Tasks
{
    public interface IOperationContext
    {
        CancellationToken CancellationToken { get; }
        IObservable<Unit> Cancelled { get; }
        IObservable<Unit> Cancelling { get; }
        void Cancel();
        void Start();
        void SetOperationAsCancelled();
    }
}