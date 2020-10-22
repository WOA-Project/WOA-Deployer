using System.Collections.Generic;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Requirements
{
    public interface IRequirementsAnalyzer
    {
        Either<ErrorList, IEnumerable<MissingRequirement>> GetRequirements(string content);
    }
}