using Deployer.Core.NetCoreApp;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Sections;
using Grace.DependencyInjection;

namespace Deployer.Gui
{
    public class Composition
    {
        private readonly DependencyInjectionContainer container;

        public Composition()
        {
            container = CompositionRoot.CreateContainer();
        }

        public MainViewModel Root => container.Locate<MainViewModel>();
    }
}