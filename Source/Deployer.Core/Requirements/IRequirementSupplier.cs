using System.Collections.Generic;
using System.Threading.Tasks;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Requirements
{
    public interface IRequirementSupplier
    {
        Task<Either<Error, IEnumerable<FulfilledRequirement>>> Satisfy(IEnumerable<MissingRequirement> requirements);
    }
}