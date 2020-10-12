using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Optional.Unsafe;

namespace Deployer.Tests
{
    public class IridioRequirementsAnalyzer : IRequirementsAnalyzer
    {
        public IEnumerable<FulfilledRequirement> GetRequirements(string content)
        {
            var pattern = @"(?i)\s*//\s*Requires\s+([A-Za-z_]+[\dA-Za-z_])\s+""(.+)""\s+as\s+""(.+)""";
            var matches = Regex.Matches(content, pattern);
            return matches.Cast<Match>()
                .Select(m => new FulfilledRequirement(m.Groups[2].Value, RequirementKind.Parse(m.Groups[1].Value).ValueOrFailure(), m.Groups[3].Value));
        }
    }
}