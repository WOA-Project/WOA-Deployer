using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core.Requirements
{
    public interface IRequirementsManager
    {
        Task<IEnumerable<FulfilledRequirement>> Satisfy(string path);
    }
}