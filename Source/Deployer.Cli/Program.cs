using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Deployer.NetFx;
using Serilog;

namespace Deployer.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new CommandLineBuilder()
                .UseDefaults()
                .UseExceptionHandler((exception, context) =>
                {
                    Log.Error(exception, "An error has occurred: {Error}",
                        exception?.InnerException?.Message ?? exception?.Message);
                })
                .Configure(CompositionRoot.CreateContainer())
                .Build();

            return await builder.InvokeAsync(args);
        }
    }
}
