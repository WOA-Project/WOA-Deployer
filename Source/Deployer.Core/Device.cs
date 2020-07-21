using System;
using System.Collections;

namespace Deployer.Core
{
    public class Device
    {
        public string Code { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }

        public static Device FromString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
