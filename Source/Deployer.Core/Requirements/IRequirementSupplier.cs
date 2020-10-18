using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core.Requirements
{
    public interface IRequirementSupplier
    {
        Task<IEnumerable<FulfilledRequirement>> Satisfy(IEnumerable<MissingRequirement> requirements);
    }
}