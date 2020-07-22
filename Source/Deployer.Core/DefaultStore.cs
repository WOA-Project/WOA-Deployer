using System.Collections.Generic;
using System.Linq;

namespace Deployer.Core
{
    public static class DefaultStore
    {
        static string Cityman = "https://www.gadgets4geeks.com.au/WebRoot/Store/Shops/gadgets4geeks/Categories/gadgets-by-brand/Microsoft/Microsoft_Lumia_950_XL/00-microsoft-lumia-950-xl-accessories_m.png";
        static string Talkman = "https://www.billiger-telefonieren.de/bilder/microsoft-lumia-950_0101w300_9112.png";
        static string Rpi3 = "https://www.adafruit.com/includes/templates/adafruit2013/images/little_pi.png";
        static string Rpi4 = "https://www.redeszone.net/app/uploads-redeszone.net/2019/07/Raspberry_Pi_4_destacada-1.png";

        private static string Emmc =
            "https://cdn1.virmach.com/wp-content/uploads/2015/08/memory-chip-processor-memory-icone-4185-128.png";

        static string MicroSD = "http://icons.iconarchive.com/icons/dakirby309/simply-styled/64/Micro-SD-Card-icon.png";

        public static DeployerStore GetDeployerStore()
        {
            var store = new DeployerStore();

            var citymanSs = new Device
            {
                Code = "CitymanSS",
                FriendlyName = "Lumia 950 XL - Single SIM",
                Icon =  Cityman,
                Name = "Cityman",
                Variant = "Single SIM",
            };

            var citymanDs = new Device
            {
                Code = "CitymanDS",
                FriendlyName = "Lumia 950 XL - Dual SIM",
                Icon =  Cityman,
                Name = "Cityman",
                Variant = "Dual SIM",
            };

            var talkmanSs = new Device
            {
                Code = "TalkmanSS",
                FriendlyName = "Lumia 950 - Single SIM",
                Icon = Talkman,
                Name = "Talkman",
                Variant = "Single SIM",
            };

            var talkmanDs = new Device
            {
                Code = "TalkmanDS",
                FriendlyName = "Lumia 950 - Dual SIM",
                Icon = Talkman,
                Name = "Talkman",
                Variant = "Dual SIM",
            };

            var rpi3 = new Device
            {
                Code = "RaspberryPi3",
                FriendlyName = "Raspberry Pi 3",
                Name = "Raspberry Pi 3",
                Icon = Rpi3,
            };

            var rpi4 = new Device
            {
                Code = "RaspberryPi4",
                FriendlyName = "Raspberry Pi 4",
                Name = "Raspberry Pi 3",
                Icon = Rpi4,
            };

            var citymans = new[] {citymanSs, citymanDs};

            var talkmans = new[] {talkmanSs, talkmanDs};

            store.Devices = citymans.Concat(talkmans).Concat(new [] {rpi3, rpi4}).ToList();

            store.Deployments = new List<Deployment>
            {
                new Deployment
                {
                    Title = "Lumia 950 XL (eMMC)",
                    Description = "Deploys WOA into the phone's internal memory",
                    Devices = citymans,
                    ScriptPath = "Devices\\Lumia\\950s\\Cityman\\Main.txt",
                    Icon = Emmc,
                },
                new Deployment
                {
                    Title = "Lumia 950 XL (MicroSD)",
                    Description = "Deploys WOA into a MicroSD card",
                    Devices = citymans,
                    ScriptPath = "Devices\\Lumia\\950s\\Cityman\\CardInstall\\Main.txt",
                    Icon = MicroSD,
                },
                new Deployment
                {
                    Title = "Lumia 950 (eMMC)",
                    Description = "Deploys WOA into the phone's internal memory",
                    Devices = talkmans,
                    ScriptPath = "Devices\\Lumia\\950s\\Talkman\\Main.txt",
                    Icon = Emmc,
                },
                new Deployment
                {
                    Title = "Raspberry Pi 3",
                    Description = "For Model B/B+",
                    Devices = new[] {rpi3},
                    ScriptPath = "Devices\\Raspberry Pi 3\\Main.txt",
                    Icon = MicroSD,
                },
                new Deployment
                {
                    Title = "Raspberry Pi 4",
                    Description = "For Model B/B+",
                    Devices = new[] {rpi4},
                    ScriptPath = "Devices\\Raspberry Pi 4\\Main.txt",
                    Icon = MicroSD,
                },
            };

            return store;
        }
    }
}