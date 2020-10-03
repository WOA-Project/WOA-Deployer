using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Deployer.Tests
{
    public class IridioRequirementsAnalyzer : IRequirementsAnalyzer
    {
        public IEnumerable<FullFilledRequirement> GetRequirements(string content)
        {
            var pattern = @"(?i)\s*//\s*Requires\s+([A-Za-z_]+[\dA-Za-z_])\s+""(.+)""\s+as\s+""(.+)""";
            var matches = Regex.Matches(content, pattern);
            return matches.Cast<Match>()
                .Select(m => new FullFilledRequirement(m.Groups[2].Value, m.Groups[1].Value, m.Groups[3].Value));
        }
    }
}