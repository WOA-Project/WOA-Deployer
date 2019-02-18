using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Gui.Common.Views;

namespace Deployer.Gui.Common.Services
{
    public interface IMarkdownDialog
    {
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}