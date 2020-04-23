using System;

namespace Deployer.Core.Interaction
{
    public interface IBusy
    {        
        IObservable<bool> IsBusyObservable { get; }
    }
}