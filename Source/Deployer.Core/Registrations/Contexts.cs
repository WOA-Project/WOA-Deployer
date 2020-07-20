using Deployer.Core.Scripting;
using Grace.DependencyInjection;
using Zafiro.Core;

namespace Deployer.Core.Registrations
{
    public class Contexts : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<OperationContext>().As<IOperationContext>().Lifestyle.SingletonPerScope();
            block.Export<OperationProgress>().As<IOperationProgress>().Lifestyle.SingletonPerScope();
        }
    }
}