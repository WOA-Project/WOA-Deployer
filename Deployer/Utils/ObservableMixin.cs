using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Deployer.Utils
{
    public static class ObservableMixin
    {
        public static IObservable<TSource> Using<TSource, TResource>(
            Func<Task<TResource>> resourceFactoryAsync,
            Func<TResource, IObservable<TSource>> observableFactory)
            where TResource : IDisposable =>
            Observable.FromAsync(resourceFactoryAsync).SelectMany(
                resource => Observable.Using(() => resource, observableFactory));

        public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(2, n));

        public static IObservable<T> RetryWithBackoffStrategy<T>(
            this IObservable<T> source,
            int retryCount = 3,
            Func<int, TimeSpan> strategy = null,
            Func<Exception, bool> retryOnError = null,
            IScheduler scheduler = null)
        {
            strategy = strategy ?? ExponentialBackoff;
            scheduler = scheduler ?? Scheduler.Default;

            if (retryOnError == null)
            {
                retryOnError = e => true;
            }

            var attempt = 0;

            return Observable.Defer(
                    () =>
                    {
                        var observable = ++attempt == 1 ? source : source.DelaySubscription(strategy(attempt - 1), scheduler);

                        return observable
                            .Select(Notification.CreateOnNext)
                            .Catch(
                                (Exception e) => retryOnError(e)
                                    ? Observable.Throw<Notification<T>>(e)
                                    : Observable.Return(Notification.CreateOnError<T>(e)));
                    })
                .Retry(retryCount)
                .Dematerialize();
        }    
    }
}