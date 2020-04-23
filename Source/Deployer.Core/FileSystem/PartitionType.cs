using System;
using System.Collections.Generic;
using System.Linq;

namespace Deployer.Core.FileSystem
{
    public class PartitionType
    {
        private static readonly Guid EspGuid = Guid.Parse("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
        private static readonly Guid BasicGuid = Guid.Parse("EBD0A0A2-B9E5-4433-87C0-68B6B72699C7");
        private static readonly Guid ReservedGuid = Guid.Parse("E3C9E316-0B5C-4DB8-817D-F92DF00215AE");
        private static readonly Guid RecoveryGuid = Guid.Parse("de94bba4-06d1-4d40-a16a-bfd50179d6ac");

        public string Code { get; }
        public string Name { get; }
        public Guid Guid { get; }

        public static readonly PartitionType Reserved = new PartitionType(nameof(Reserved), "Reserved", ReservedGuid);
        public static readonly PartitionType Esp = new PartitionType(nameof(Esp), "EFI System Partition", EspGuid);
        public static readonly PartitionType Basic = new PartitionType(nameof(Basic), "Basic", BasicGuid);
        public static readonly PartitionType Recovery = new PartitionType(nameof(Recovery) , "Recovery", RecoveryGuid);

        private static readonly IDictionary<Guid, PartitionType> GuidToTypeDictionary = new Dictionary<Guid, PartitionType>()
        {
            { EspGuid, Esp},
            { BasicGuid, Basic },
            { ReservedGuid, Reserved },
            { RecoveryGuid, Recovery },
        };

        private PartitionType(string code, string name, Guid guid)
        {
            Code = code;
            Name = name;
            Guid = guid;
        }

        public static PartitionType FromGuid(Guid guid)
        {
            if (GuidToTypeDictionary.TryGetValue(guid, out var type))
            {
                return type;
            }

            return new PartitionType("Unknown", "Unknown type", guid);
        }

        public static PartitionType FromString(string str)
        {
            var partType = GuidToTypeDictionary.Values.FirstOrDefault(pair =>
                string.Equals(pair.Code, str, StringComparison.InvariantCultureIgnoreCase));

            return partType;
        }

        protected bool Equals(PartitionType other)
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

            return Equals((PartitionType) obj);
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