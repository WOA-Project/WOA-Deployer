using System;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace Deployer.Utils
{
    public static class SerialDisposableExtensions
    {
        /// <summary>
        /// Uses the double-indirection pattern to assign the disposable returned by the specified <paramref name="factory"/>
        /// to the <see cref="SerialDisposable.Disposable"/> property of the specified <paramref name="disposable"/>.
        /// </summary>
        /// <remarks>
        /// The double-indirection pattern avoids a race condition that can occur when the <paramref name="factory"/> 
        /// has a side-effect that causes the <see cref="SerialDisposable.Disposable"/> property of the specified 
        /// <paramref name="disposable"/> to be assigned before the <paramref name="factory"/> returns its disposable.
        /// This pattern ensures that the disposable returned by the <paramref name="factory"/> does not replace the 
        /// disposable that was assigned by the <paramref name="factory"/>.
        /// </remarks>
        /// <param name="disposable">The object to which the disposable returned by the specified <paramref name="factory"/> is assigned.</param>
        /// <param name="factory">Returns an <see cref="IDisposable"/> that is assigned to the specified <paramref name="disposable"/>.</param>
        /// <seealso href="http://social.msdn.microsoft.com/Forums/en-IE/rx/thread/4e15feae-9c4c-4962-af32-95dde1420dda#4d5fe8c8-e5e8-4ee7-93ca-b48b6a56b8af">
        /// Double indirection pattern example in Rx
        /// </seealso>
        public static void SetDisposableIndirectly(this SerialDisposable disposable, Func<IDisposable> factory)
        {
            var indirection = new SingleAssignmentDisposable();

            disposable.Disposable = indirection;

            indirection.Disposable = factory();
        }
    }
}