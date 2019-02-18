using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Gui.Common.Services;
using Deployer.Gui.Common.Views;

namespace Deployer.Gui.Common
{
    public interface IDialog
    {
        Task ShowAlert(object owner, string title, string text);
        Task<DialogResult> ShowConfirmation(object owner, string title, string text);
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}