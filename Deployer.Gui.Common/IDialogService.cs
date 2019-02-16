using System.Threading.Tasks;
using Deployer.Gui.Common.Services;

namespace Deployer.Gui.Common
{
    public interface IDialogService
    {
        Task ShowAlert(object owner, string title, string text);
        Task<DialogResult> ShowConfirmation(object owner, string title, string text);
    }
}