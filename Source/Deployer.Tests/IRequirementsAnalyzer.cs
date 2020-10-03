using System.Collections.Generic;

namespace Deployer.Tests
{
    public interface IRequirementsAnalyzer
    {
        IEnumerable<FullFilledRequirement> GetRequirements(string content);
    }
}