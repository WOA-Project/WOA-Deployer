using Deployer.FileSystem;
using Deployer.Lumia.NetFx.PhoneMetadata;
using Deployer.NetFx;
using Grace.DependencyInjection;

namespace Deployer.Lumia.Gui.Registrations
{
    public class Phone : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<PhoneInfoReader>().As<IPhoneInfoReader>().Lifestyle.Singleton();
            block.Export<PhoneModelInfoInfoReader>().As<IPhoneModelInfoReader>().Lifestyle.Singleton();
            block.Export<Lumia.Phone>().As<IPhone>().Lifestyle.Singleton();
            block.Export<DiskRoot>().As<IDiskRoot>().Lifestyle.Singleton();
        }
    }
}