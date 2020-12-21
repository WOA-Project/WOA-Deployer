using System;
using System.Threading.Tasks;
using Deployer.Console;
using Deployer.Core.Console;
using Deployer.Functions;
using Zafiro.Core.Patterns.Either;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var deployer = new WoaDeployerConsole(new[] { typeof(Anchor).Assembly });

            using (deployer.Messages.Subscribe(Console.WriteLine))
            {
                using (new ConsoleProgressUpdater(deployer.OperationProgress))
                {
                    var result = await deployer.Run("D:\\Repos\\WOA-Project\\Deployment-Feed\\Devices\\Lumia\\950s\\Cityman\\Main.txt");
                    Console.WriteLine(result
                        .MapRight(s => "Success!")
                        .Handle(x => $"Failed with errors: {x}"));
                }
            }
        }
    }
}