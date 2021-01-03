using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Deployer.Gui
{
    public static class ObservableExtensions
    {
        public static IDisposable OnSuccess<T>(this IObservable<T> observable, Func<Task> func)
        {
            return observable.SelectMany(async x =>
            {
                await func();
                return Unit.Default;
            }).Subscribe();
        }
    }
}