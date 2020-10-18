using Grace.DependencyInjection;
using Iridio.Parsing;
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
        }
    }
}