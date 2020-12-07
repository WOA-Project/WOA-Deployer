using Deployer.Core.Services;
using Deployer.Filesystem;
using Grace.DependencyInjection;

namespace Deployer.NetFx.Registrations
{
    public class Common : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<FileSystem>().As<IFileSystem>().Lifestyle.Singleton();
            block.Export<ImageFlasher>().As<IImageFlasher>().Lifestyle.Singleton();
            block.Export<DismImageService>().As<IWindowsImageService>().Lifestyle.Singleton();
        }
    }
}