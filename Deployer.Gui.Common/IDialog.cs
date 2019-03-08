using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Gui.Services;

namespace Deployer.Gui
{
    public interface IDialog
    {
        Task ShowAlert(object owner, string title, string text);
        Task<DialogResult> ShowConfirmation(object owner, string title, string text);
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}