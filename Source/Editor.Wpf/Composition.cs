using Grace.DependencyInjection;

namespace Editor.Wpf
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