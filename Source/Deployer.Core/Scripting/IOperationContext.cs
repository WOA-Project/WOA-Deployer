using System;
using System.Reactive;
using System.Threading;

namespace Deployer.Core.Scripting
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