using ConsoleApp1;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Net4x;
using Deployer.NetFx;
using Grace.DependencyInjection;

namespace Deployer.Console
{
    public class WoaDeployerConsole : WoaDeployerBase
    {
        protected override void ExportSpecificDependencies(IExportRegistrationBlock block)
        {
            block.ExportFactory(() => new ConsoleRequirementsManager()).As<IRequirementsManager>();
            block.Export<ConsoleMarkdownService>().As<IMarkdownService>();
        }
    }
}