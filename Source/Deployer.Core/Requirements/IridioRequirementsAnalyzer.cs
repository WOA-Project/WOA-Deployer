using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Optional.Unsafe;

namespace Deployer.Core.Requirements
{
    public class IridioRequirementsAnalyzer : IRequirementsAnalyzer
    {
        public IEnumerable<MissingRequirement> GetRequirements(string content)
        {
            var pattern = @"(?i)\s*//\s*Requires\s+([A-Za-z_]+[\dA-Za-z_])\s+""(.+)""\s+as\s+""(.+)""";
            var matches = Regex.Matches(content, pattern);
            return matches.Cast<Match>()
                .Select(m => new MissingRequirement(m.Groups[2].Value, RequirementKind.Parse(m.Groups[1].Value).ValueOrFailure(), m.Groups[3].Value));
        }
    }
}