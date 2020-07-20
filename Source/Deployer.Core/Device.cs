using System;
using System.Collections;
using System.Collections.Generic;

namespace Deployer.Core
{
    public class Deployment
    {
        public Device Device { get; set; }
        public string ScriptPath { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Device
    {
        public string Code { get; set; }
        public string FriendlyName { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }

        public static Device FromString(string value)
        {
            throw new NotImplementedException();
        }
    }

    public class DeployerStore 
    {
        public IEnumerable<Device> Devices { get; set; }
        public IEnumerable<Deployment> Deployments { get; set; }
    }
}
