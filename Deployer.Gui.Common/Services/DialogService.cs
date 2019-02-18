using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Gui.Common.Views;
using MahApps.Metro.Controls.Dialogs;

namespace Deployer.Gui.Common.Services
{
    public class DialogService : IDialogService, IMarkdownDialog
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