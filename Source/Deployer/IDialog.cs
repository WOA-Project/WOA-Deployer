using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IDialog
    {
        Task<Option> Pick(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "");
        Task<DialogResult> Show(string key, object context);
    }
}