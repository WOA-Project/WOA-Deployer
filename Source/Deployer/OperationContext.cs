using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Deployer.Tasks;

namespace Deployer
{
    public class OperationContext: IOperationContext
    {
        private CancellationTokenSource cancellationTokenSource;
        private readonly ISubject<Unit> cancellingSubject = new Subject<Unit>();
        private readonly ISubject<Unit> cancelledSubject = new Subject<Unit>();
        public IObservable<Unit> Cancelled => cancelledSubject;
        public IObservable<Unit> Cancelling => cancellingSubject;

        public void Cancel()
        {
            if (cancellationTokenSource == null)
            {
                throw new InvalidOperationException("No operation has been started. Please, use the Start method before cancelling.");
            }

            cancellationTokenSource.Cancel();
            cancellingSubject.OnNext(Unit.Default);
        }

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void SetOperationAsCancelled()
        {
            cancelledSubject.OnNext(Unit.Default);
        }

        public CancellationToken CancellationToken => cancellationTokenSource?.Token ?? default(CancellationToken);
    }
}