using Deployer.Core.Compiler;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Parsing;
using Iridio.Runtime;
using IParser = Iridio.Parsing.IParser;
using Parser = Iridio.Parsing.Parser;

namespace Deployer.Core.Registrations
{
    public class Iridio : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<Preprocessor>().As<IPreprocessor>().Lifestyle.Singleton();
            block.Export<Parser>().As<IParser>().Lifestyle.Singleton();
            block.Export<Binder>().As<IBinder>().Lifestyle.Singleton();
            block.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
            block.Export<ScriptRunner>().As<IScriptRunner>().Lifestyle.Singleton();
        }
    }
}
