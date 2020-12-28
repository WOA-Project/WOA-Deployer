using System.Collections.Generic;
using System.Linq;
using Deployer.Core.DeploymentLibrary;

namespace Deployer.Core
{
    public static class DefaultStore
    {
        static string Cityman = "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/lumia950xl.png?raw=true";
        static string Talkman = "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/lumia950.png?raw=true";
        static string Rpi3 = "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/rpi3.png?raw=true";
        static string Rpi4 = "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/rpi4.png?raw=true";

        private static string Emmc =
            "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/emmc.png?raw=true";

        static string MicroSD = "https://github.com/WOA-Project/Deployment-Feed/blob/master/Assets/microsd.png?raw=true";

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

            store.Devices.ForEach((device, id) => device.Id = id + 1);

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

            store.Deployments.ForEach((d, id) => d.Id = id + 1);


            return store;
        }
    }
}