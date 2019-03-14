using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IPrompt
    {
        Task<Option> PickOptions(string markdown, IEnumerable<Option> options);
    }
}