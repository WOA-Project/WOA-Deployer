using System.IO;
using Deployer.Core.Compiler;
using Deployer.Core.FileSystem;
using Grace.DependencyInjection;
using SimpleScript;

namespace Deployer.Core.Registrations
{
    public class Scripting : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<Parser>().As<IParser>().Lifestyle.Singleton();
            block.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
            block.Export<Runner>().As<IRunner>().Lifestyle.Singleton();
        }
    }
}