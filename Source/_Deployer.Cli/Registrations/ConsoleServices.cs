using Deployer.Cli.Services;
using Deployer.Core;
using Deployer.Core.Services;
using Grace.DependencyInjection;
using Zafiro.Core.UI;

namespace Deployer.Cli.Registrations
{
    public class ConsoleServices : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            block.Export<DialogService>().As<IDialogService>().Lifestyle.Singleton();
            block.Export<DictionaryBasedSatisfier>().As<IRequirementSatisfier>().Lifestyle.Singleton();
        }
    }
}