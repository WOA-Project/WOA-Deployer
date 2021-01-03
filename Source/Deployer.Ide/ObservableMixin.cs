using System;
using System.Reactive.Linq;

namespace Deployer.Ide
{
    public static class ObservableMixin
    {
        public static IObservable<bool> Invert(this IObservable<bool> self)
        {
            return self.Select(b => !b);
        }
    }
}