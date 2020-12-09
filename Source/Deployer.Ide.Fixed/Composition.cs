using Grace.DependencyInjection;

namespace Deployer.Ide
{
    public class Composition
    {
        private readonly DependencyInjectionContainer container;

        public Composition()
        {
            container = CompositionRoot.CreateContainer();
        }

        public MainViewModel Root
        {
            get
            {
                return container.Locate<MainViewModel>();
            }
        }
    }
}