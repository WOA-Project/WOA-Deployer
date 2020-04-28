using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IRequirementSatisfier
    {
        Task<bool> Satisfy(IDictionary<string, object> unsatisfied);
    }
}