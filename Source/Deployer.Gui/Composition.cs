using System;
using Deployer.Core.NetCoreApp;
using Deployer.Core.Registrations;
using Deployer.Gui.ViewModels.Sections;
using Grace.DependencyInjection;
using Zafiro.Core;

namespace Deployer.Gui
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

        private static XmlDeviceRepository XmlDeviceRepository(IDownloader downloader)
        {
            var definition = "https://raw.githubusercontent.com/WOA-Project/Deployment-Feed/master/Deployments.xml";
            return new XmlDeviceRepository(new Uri(definition), downloader);
        }
    }
}