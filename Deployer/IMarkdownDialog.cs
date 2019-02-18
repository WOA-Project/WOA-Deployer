using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IMarkdownDialog
    {
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}