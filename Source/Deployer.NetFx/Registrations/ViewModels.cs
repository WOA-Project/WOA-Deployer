using Deployer.Lumia.Gui.ViewModels;
using Deployer.UI;
using Grace.DependencyInjection;

namespace Deployer.Lumia.Gui.Registrations
{
    public class ViewModels : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.ExportAssemblies(new[] { typeof(MainViewModel).Assembly })
                .Where(y => typeof(ISection).IsAssignableFrom(y))
                .ByInterface<ISection>()
                .ByInterface<IBusy>()
                .ByType()
                .ExportAttributedTypes()
                .Lifestyle.Singleton();
        }
    }
}