using Deployer.Core;
using Grace.DependencyInjection;

namespace Deployer.Lumia.Registrations
{
    public class Common : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<PhoneModelInfoInfoReader>().As<IPhoneModelInfoReader>();
            block.Export<PhoneInfoReader>().As<IPhoneInfoReader>();
            block.Export<LumiaContextualizer>().As<IContextualizer>();
            //block.Export<LumiaDetector>().As<IDetector>();
        }
    }
}