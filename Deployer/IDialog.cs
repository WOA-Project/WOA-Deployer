using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IDialog
    {
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
        Task<DialogResult> Show(string key, object context);

    }
}