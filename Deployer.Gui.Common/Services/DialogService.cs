using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Deployer.Gui.Common.Services
{
    public class DialogService : IDialogService
    {
        private readonly IDialogCoordinator coordinator;

        public DialogService(IDialogCoordinator coordinator)
        {
            this.coordinator = coordinator;
        }

        public Task ShowAlert(object owner, string title, string text)
        {
            return coordinator.ShowMessageAsync(owner, title, text);
        }

        public async Task<DialogResult> ShowConfirmation(object owner, string title, string text)
        {
            var result = await coordinator.ShowMessageAsync(owner, title, text, MessageDialogStyle.AffirmativeAndNegative);
            return result == MessageDialogResult.Affirmative ? DialogResult.Yes : DialogResult.No;
        }
    }
}