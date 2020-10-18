using System.Collections.Generic;

namespace Deployer.Core.Requirements
{
    public interface IRequirementsAnalyzer
    {
        IEnumerable<MissingRequirement> GetRequirements(string content);
    }
}