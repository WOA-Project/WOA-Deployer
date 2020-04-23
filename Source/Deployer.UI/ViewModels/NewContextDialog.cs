using System.Collections.Generic;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using Zafiro.Core.UI;

namespace Deployer.UI.ViewModels
{
    public class NewContextDialog : IDialogService
    {
        private readonly object context;

        public NewContextDialog(object context)
        {
            this.context = context;
        }

        public Task Show(string title, string content)
        {
            return DialogCoordinator.Instance.ShowMessageAsync(context, title, content);
        }

        public Task<Option> Pick(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            throw new System.NotImplementedException();
        }
    }
}