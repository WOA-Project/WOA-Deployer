using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Deployer.NetFx;

namespace Deployer.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new CommandLineBuilder()
                .UseDefaults()
                //.UseExceptionHandler((exception, context) =>
                //{
                //    Log.Error(exception, "An error has occurred: {Error}",
                //        exception?.InnerException?.Message ?? exception?.Message);
                //})
                .Configure(CompositionRoot.CreateContainer())
                .Build();

            var invokeAsync = await builder.InvokeAsync(args);
        }
    }
}
