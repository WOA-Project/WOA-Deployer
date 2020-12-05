using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Requirements;
using Zafiro.Core.Patterns.Either;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var deployer = new WoaDeployer(new MyRequirementsManager());
            var result = await deployer.Run("C:\\Users\\SuperJMN\\Desktop\\Test.txt");
            Console.WriteLine(result
                .MapRight(s => "Success!")
                .Handle(x => $"Failed with errors: {x}"));
        }
    }

    internal class MyRequirementsManager : IRequirementsManager
    {
        public async Task<Either<ErrorList, IEnumerable<FulfilledRequirement>>> Satisfy(string path)
        {
            return Either.Success<ErrorList, IEnumerable<FulfilledRequirement>>(Enumerable.Empty<FulfilledRequirement>());
        }
    }
}
