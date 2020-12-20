using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Core.Requirements;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Console
{
    internal class ConsoleRequirementsManager : IRequirementsManager
    {
        public async Task<Either<ErrorList, IEnumerable<FulfilledRequirement>>> Satisfy(string path)
        {
            return Either.Success<ErrorList, IEnumerable<FulfilledRequirement>>(new[]
            {
                new FulfilledRequirement("Disk", 4),
                new FulfilledRequirement("WimFilePath", "F:\\sources\\install.wim"),
                new FulfilledRequirement("WimFileIndex", 1),
            });
        }
    }
}