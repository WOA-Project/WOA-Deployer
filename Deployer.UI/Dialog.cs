using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.UI.Views;

namespace Deployer.UI
{
    public class Dialog : IDialog
    {
        public async Task<Option> Pick(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            var markdownViewerWindow = new MarkdownViewerWindow();
            Option option;
            using (var viewModel = new MarkupMessageViewModel(title, markdown, options, markdownViewerWindow, assetBasePath))
            {
                markdownViewerWindow.DataContext = viewModel;
                var wnd = markdownViewerWindow;
                await wnd.ShowDialogAsync();
                option = viewModel.SelectedOption;
            }

            return option;
        }

        public async Task<DialogResult> Show(string key, object context)
        {
            var dialogWindow = new DialogWindow();
            var options = new List<Option>
            {
                new Option("OK", OptionValue.OK),
                new Option("Cancel", OptionValue.Cancel)
            };

            var dialogViewModel = new DialogViewModel("Deployment options", context, options, dialogWindow);
            dialogWindow.DataContext = dialogViewModel;

            var confirmed = (await dialogWindow.ShowDialogAsync()).HasValue && dialogViewModel.SelectedOption?.OptionValue == OptionValue.OK;
            dialogWindow.Close();

            return confirmed ? DialogResult.Yes : DialogResult.No;
        }
    }
}