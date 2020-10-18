using System;
using System.Threading.Tasks;
using Zafiro.Core.UI.Interaction;

namespace Deployer.Core.Requirements
{
    public interface IViewRegistry
    {
        Task Popup<T>(string key, string title, T vm, Action<PopupConfiguration<T>> configuration);
    }
}