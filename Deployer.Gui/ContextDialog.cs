using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Deployer.UI
{
    public class ContextDialog : IContextDialog
    {
        private readonly IDialogCoordinator coordinator;

        public ContextDialog(IDialogCoordinator coordinator)
        {
            this.coordinator = coordinator;
        }

        public Task ShowAlert(object owner, string title, string text)
        {
            return coordinator.ShowMessageAsync(owner, title, text);
        }

        public async Task<DialogResult> ShowOkCancel(object owner, string title, string text)
        {
            var result = await coordinator.ShowMessageAsync(owner, title, text, MessageDialogStyle.AffirmativeAndNegative);
            return result == MessageDialogResult.Affirmative ? DialogResult.Yes : DialogResult.No;
        }

        public async Task<DialogResult> ShowYesNo(object owner, string title, string text, bool isYesDefault)
        {
            var options = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                DefaultButtonFocus = isYesDefault ? MessageDialogResult.Affirmative : MessageDialogResult.Negative,
            };
            var result = await coordinator.ShowMessageAsync(owner, title, text, MessageDialogStyle.AffirmativeAndNegative, options);
            return result == MessageDialogResult.Affirmative ? DialogResult.Yes : DialogResult.No;
        }
    }
}