using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Net4x;
using Grace.DependencyInjection;

namespace Deployer.Console
{
    public class WoaDeployerConsole : WoaDeployer
    {
        protected override void ExportSpecificDependencies(IExportRegistrationBlock block)
        {
            block.ExportFactory(() => new ConsoleRequirementsManager()).As<IRequirementsManager>();
            block.Export<ConsoleMarkdownService>().As<IMarkdownService>();
        }
    }
}