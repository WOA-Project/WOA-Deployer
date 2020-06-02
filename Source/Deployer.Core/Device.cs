using System;
using System.Linq;

namespace Deployer.Core
{
    public class Device
    {
        public string Code { get; }
        public string FriendlyName { get; }
        public string[] Identifier { get; }

        public Device(string code, string friendlyName, params string[] identifier)
        {
            Code = code;
            FriendlyName = friendlyName;
            Identifier = identifier;
        }

        public static readonly Device CitymanSs = new Device(nameof(CitymanSs), "Lumia 950 XL - Single SIM", "Lumia", "Cityman", "SingleSIM");
        public static readonly Device CitymanDs = new Device(nameof(CitymanDs), "Lumia 950 XL - Dual SIM","Lumia", "Cityman", "DualSIM");
        public static readonly Device TalkmanSs = new Device(nameof(TalkmanSs), "Lumia 950 - Single SIM", "Lumia", "Talkman", "SingleSIM");
        public static readonly Device TalkmanDs = new Device(nameof(TalkmanDs), "Lumia 950 - Dual SIM", "Lumia", "Talkman", "DualSIM");
        public static readonly Device RaspberryPi3 = new Device(nameof(RaspberryPi3), "Raspberry Pi 3", "Raspberry Pi", "3");
        public static readonly Device RaspberryPi4 = new Device(nameof(RaspberryPi4), "Raspberry Pi 4", "Raspberry Pi", "4");

        public static readonly Device[] KnownDevices = {CitymanDs, CitymanSs, TalkmanDs, TalkmanSs, RaspberryPi3, RaspberryPi4};

        protected bool Equals(Device other)
        {
            return Identifier.Equals(other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Device) obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public static Device FromString(string deviceName)
        {
            var firstOrDefault = KnownDevices.FirstOrDefault(x => string.Equals(deviceName, x.Code, StringComparison.OrdinalIgnoreCase));
            if (firstOrDefault is null)
            {
                throw new ArgumentOutOfRangeException("deviceName", $@"The device {deviceName} isn't supported");
            }
            return firstOrDefault;
        }

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
