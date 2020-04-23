using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Zafiro.Core;
using Zafiro.Core.UI;

namespace Deployer.Cli.Services
{
    public class DialogService : IDialogService
    {
        public Task Notice(string title, string content)
        {
            Log.Information(title + ":" + content);
            return Task.CompletedTask;
        }

        public Task<Option> Interaction(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            throw new System.NotImplementedException();
        }
    }
}