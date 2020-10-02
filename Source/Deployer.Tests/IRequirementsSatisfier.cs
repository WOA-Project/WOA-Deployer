using System.Collections.Generic;

namespace Deployer.Tests
{
    public interface IRequirementsSatisfier
    {
        IEnumerable<RequirementSpecification> Satisfy(IEnumerable<MissingRequirement> content);
    }
}