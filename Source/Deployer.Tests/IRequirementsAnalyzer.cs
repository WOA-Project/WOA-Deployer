using System.Collections.Generic;

namespace Deployer.Tests
{
    public interface IRequirementsAnalyzer
    {
        IEnumerable<FulfilledRequirement> GetRequirements(string content);
    }
}