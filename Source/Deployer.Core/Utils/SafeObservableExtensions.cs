using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;

namespace Deployer.Core.Utils
{
    public static class SafeObservableExtensions
    {
        /// <summary>
        /// Subscribes an element handler and an exception handler to the specified <paramref name="source"/>, re-routing synchronous 
        /// exceptions during invocation of the <strong>Subscribe</strong> method to the observer's <strong>OnError</strong> channel.
        /// This method is typically used when writing query operators.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IDisposable SubscribeSafe<T>(this IObservable<T> source, Action<T> onNext,
            Action<Exception> onError)
        {
            return source.SubscribeSafe(Observer.Create<T>(onNext, onError));
        }

        /// <summary>
        /// Subscribes an element handler, an exception handler and a completion handler to the specified <paramref name="source"/>, 
        /// re-routing synchronous exceptions during invocation of the <strong>Subscribe</strong> method to the observer's <strong>OnError</strong> channel.
        /// This method is typically used when writing query operators.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IDisposable SubscribeSafe<T>(this IObservable<T> source, Action<T> onNext,
            Action<Exception> onError, Action onCompleted)
        {
            Contract.Requires(source != null);
            Contract.Requires(onNext != null);
            Contract.Requires(onError != null);
            Contract.Requires(onCompleted != null);
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            return source.SubscribeSafe(Observer.Create<T>(onNext, onError, onCompleted));
        }
    }
}