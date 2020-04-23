using Deployer.Core.Services;
using Grace.DependencyInjection;

namespace Deployer.Core.Registrations
{
    public class Deployment : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<BootCreator>().As<IBootCreator>().Lifestyle.Singleton();
            block.ExportFactory((string store) => new BcdInvoker(store)).As<IBcdInvoker>();
            block.Export<DoNothingBcdConfigurator>().As<IBcdConfigurator>().Lifestyle.Singleton();
        }
    }
}