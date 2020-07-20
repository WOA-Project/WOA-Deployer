using System;
using System.Linq;

namespace Deployer.Core
{
    public class Device
    {
        public string Code { get; set; }
        public string FriendlyName { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public string DeploymentScriptPath { get; set; }
        public string Icon { get; set; }

        public static Device FromString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
