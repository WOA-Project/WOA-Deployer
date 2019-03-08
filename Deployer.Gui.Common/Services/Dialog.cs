using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Gui.Views;
using MahApps.Metro.Controls.Dialogs;

namespace Deployer.Gui.Services
{
    public class Dialog : IDialog, IMarkdownDialog
    {
        private readonly IDialogCoordinator coordinator;

        public Dialog(IDialogCoordinator coordinator)
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

        public async Task<Option> PickOptions(string markdown, IEnumerable<Option> options)
        {
            var markdownViewerWindow = new MarkdownViewerWindow();
            Option option;
            using (var viewModel = new AutoMessageViewModel(markdown, options, markdownViewerWindow))
            {
                markdownViewerWindow.DataContext = viewModel;

                var wnd = markdownViewerWindow;
                await wnd.ShowDialogAsync();
                option = viewModel.SelectedOption;
            }

            return option;
        }
    }
}