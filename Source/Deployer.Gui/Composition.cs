using Deployer.NetFx;
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