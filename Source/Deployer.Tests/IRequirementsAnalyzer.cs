using System.Collections.Generic;

namespace Deployer.Tests
{
    public interface IRequirementsAnalyzer
    {
        IEnumerable<RequirementSpecification> GetRequirements(string content);
    }
}