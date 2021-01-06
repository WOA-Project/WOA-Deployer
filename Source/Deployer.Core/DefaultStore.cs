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
                Code = "Cityman",
                FriendlyName = "Lumia 950 XL",
                Icon =  Cityman,
                Name = "Lumia 950 XL",
            };

            var talkmanSs = new Device
            {
                Code = "Talkman",
                FriendlyName = "Lumia 950",
                Icon = Talkman,
                Name = "Lumia 950",
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
                Name = "Raspberry Pi 4",
                Icon = Rpi4,
            };

            var citymans = new[] {citymanSs, };

            var talkmans = new[] {talkmanSs, };

            store.Devices = citymans.Concat(talkmans).Concat(new [] {rpi3, rpi4}).ToList();

            store.Devices.ForEach((device, id) => device.Id = id + 1);

            store.Deployments = new List<Deployment>
            {
                new Deployment
                {
                    Title = "Standard",
                    Description = "Deploys WOA into the phone's internal memory",
                    Devices = citymans,
                    ScriptPath = "Devices\\Lumia\\950s\\Cityman\\Main.txt",
                    Icon = Emmc,
                },
                new Deployment
                {
                    Title = "Standard",
                    Description = "Deploys WOA into the phone's internal memory",
                    Devices = talkmans,
                    ScriptPath = "Devices\\Lumia\\950s\\Talkman\\Main.txt",
                    Icon = Emmc,
                },
                new Deployment
                {
                    Title = "Standard",
                    Description = "For Model B/B+",
                    Devices = new[] {rpi3},
                    ScriptPath = "Devices\\Raspberry Pi\\3\\Main.txt",
                    Icon = MicroSD,
                },
                new Deployment
                {
                    Title = "Standard",
                    Description = "For Model B",
                    Devices = new[] {rpi4},
                    ScriptPath = "Devices\\Raspberry Pi\\4\\Main.txt",
                    Icon = MicroSD,
                },
            };

            store.Deployments.ForEach((d, id) => d.Id = id + 1);


            return store;
        }
    }
}