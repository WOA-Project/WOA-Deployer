using System.Collections.Generic;

namespace Deployer.Tests
{
    public interface IRequirementsSatisfier
    {
        IEnumerable<FullFilledRequirement> Satisfy(IEnumerable<MissingRequirement> content);
    }
}