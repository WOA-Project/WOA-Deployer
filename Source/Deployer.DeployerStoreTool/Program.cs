using System;
using System.Collections.Generic;
using System.IO;
using Deployer.Core;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace Deployer.DeployerStoreTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new DeployerStore();
            var citymanSs = new Device()
            {
                Code = "CitymanSS",
                FriendlyName = "Lumia 950 XL - Single SIM",
                Icon =
                    "https://www.gadgets4geeks.com.au/WebRoot/Store/Shops/gadgets4geeks/Categories/gadgets-by-brand/Microsoft/Microsoft_Lumia_950_XL/00-microsoft-lumia-950-xl-accessories_m.png",
                Model = "Cityman",
                Variant = "Single SIM",
            };
            var citymanDs = new Device()
            {
                Code = "CitymanDS",
                FriendlyName = "Lumia 950 XL - Dual SIM",
                Icon =
                    "https://www.gadgets4geeks.com.au/WebRoot/Store/Shops/gadgets4geeks/Categories/gadgets-by-brand/Microsoft/Microsoft_Lumia_950_XL/00-microsoft-lumia-950-xl-accessories_m.png",
                Model = "Cityman",
                Variant = "Dual SIM",
            };

            var rpi3 = new Device()
            {
                Code = "RaspberryPi3",
                FriendlyName = "Raspberry Pi 3",
                Icon =
                    "https://files1.element14.com/community/themes/images/raspberrypi/faq_pi3.png",
            };

            store.Devices = new List<Device>()
            {
                citymanSs,
                citymanDs,
                rpi3
            };

            store.Deployments = new List<Deployment>()
            {
                new Deployment()
                {
                    Description = "MicroSD Deployment",
                    Device = citymanDs,
                    ScriptPath = "Devices\\Lumia\\Cityman\\DualSim\\Main.txt",
                }
            };

            var serializer = new ConfigurationContainer()
                .EnableReferences()
                .UseOptimizedNamespaces()
                .Create();

            var serialized = serializer.Serialize(store);

            File.WriteAllText("output.xml", serialized);
        }
    }
}
