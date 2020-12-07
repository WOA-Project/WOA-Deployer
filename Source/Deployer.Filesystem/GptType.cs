using System;
using System.Collections.Generic;
using System.Linq;

namespace Deployer.Filesystem
{
    public class GptType
    {
        private static readonly Guid EspGuid = Guid.Parse("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
        private static readonly Guid BasicGuid = Guid.Parse("EBD0A0A2-B9E5-4433-87C0-68B6B72699C7");
        private static readonly Guid ReservedGuid = Guid.Parse("E3C9E316-0B5C-4DB8-817D-F92DF00215AE");
        private static readonly Guid RecoveryGuid = Guid.Parse("de94bba4-06d1-4d40-a16a-bfd50179d6ac");

        public string Code { get; }
        public string Name { get; }
        public Guid Guid { get; }

        public static readonly GptType Reserved = new GptType(nameof(Reserved), "Reserved", ReservedGuid);
        public static readonly GptType Esp = new GptType(nameof(Esp), "EFI System Partition", EspGuid);
        public static readonly GptType Basic = new GptType(nameof(Basic), "Basic", BasicGuid);
        public static readonly GptType Recovery = new GptType(nameof(Recovery) , "Recovery", RecoveryGuid);

        private static readonly IDictionary<Guid, GptType> GuidToTypeDictionary = new Dictionary<Guid, GptType>()
        {
            { EspGuid, Esp},
            { BasicGuid, Basic },
            { ReservedGuid, Reserved },
            { RecoveryGuid, Recovery },
        };

        private GptType(string code, string name, Guid guid)
        {
            Code = code;
            Name = name;
            Guid = guid;
        }

        public static GptType FromGuid(Guid guid)
        {
            if (GuidToTypeDictionary.TryGetValue(guid, out var type))
            {
                return type;
            }

            return new GptType("Unknown", "Unknown type", guid);
        }

        public static GptType FromString(string str)
        {
            var partType = GuidToTypeDictionary.Values.FirstOrDefault(pair =>
                string.Equals(pair.Code, str, StringComparison.InvariantCultureIgnoreCase));

            return partType;
        }

        protected bool Equals(GptType other)
        {
            return Guid.Equals(other.Guid);
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

            return Equals((GptType) obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} - {nameof(Guid)}: {Guid}";
        }
    }
}