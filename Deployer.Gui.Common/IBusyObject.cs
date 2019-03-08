using System;

namespace Deployer.Gui
{
    public interface IBusy
    {
        IObservable<bool> IsBusyObservable { get; }
    }
}