using System;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Console;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Filesystem;
using Deployer.NetFx;
using Zafiro.Core.Patterns.Either;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var deployer = new WoaDeployer(block =>
            {
                block.Export<FileSystem>().As<IFileSystem>().Lifestyle.Singleton();
                block.ExportFactory(() => new ConsoleRequirementsManager()).As<IRequirementsManager>();
                block.Export<FileSystem>().As<IFileSystem>();
                block.Export<ConsoleMarkdownService>().As<IMarkdownService>();
                block.Export<ShellOpen>().As<IShellOpen>().Lifestyle.Singleton();
            });

            using (deployer.Messages.Subscribe(Console.WriteLine))
            {
                using (new ConsoleProgressUpdater(deployer.OperationProgress))
                {
                    var result = await deployer.Run("C:\\Users\\SuperJMN\\Desktop\\Test.txt");
                    Console.WriteLine(result
                        .MapRight(s => "Success!")
                        .Handle(x => $"Failed with errors: {x}"));
                }
            }
        }
    }
}
