using System;

namespace Deployer.UI
{
    public interface IBusy
    {        
        IObservable<bool> IsBusyObservable { get; }
    }
}