using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.UI
{
    public interface IDialog
    {
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}