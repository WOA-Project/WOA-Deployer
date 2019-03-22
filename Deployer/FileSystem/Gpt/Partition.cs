using System;
using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public class Partition
    {
        private readonly uint bytesPerSector;
        private ulong firstSector;
        private ulong lastSector;
        private ulong sizeInSectors;

        public Partition(string name, PartitionType partitionType, uint number, uint bytesPerSector)
        {
            PartitionType = partitionType;
            Number = number;
            Name = name;
            this.bytesPerSector = bytesPerSector;
        }

        public ulong Attributes { get; set; }
        public Guid PartitionGuid { get; set; }

        public PartitionType PartitionType { get; set; }
        public uint Number { get; }

        public string Name { get; }

        private ByteSize Size => new ByteSize(SizeInSectors * bytesPerSector);

        private ulong SizeInSectors
        {
            get
            {
                if (sizeInSectors != 0)
                {
                    return sizeInSectors;
                }

                return LastSector - FirstSector + 1;
            }
            set
            {
                sizeInSectors = value;
                if (FirstSector != 0)
                {
                    LastSector = FirstSector + sizeInSectors - 1;
                }
            }
        }

        public ulong FirstSector // 0x08
        {
            get => firstSector;
            set
            {
                firstSector = value;
                if (sizeInSectors != 0)
                {
                    lastSector = FirstSector + sizeInSectors - 1;
                }
            }
        }

        public ulong LastSector // 0x08
        {
            get => lastSector;
            set
            {
                lastSector = value;
                sizeInSectors = 0;
            }
        }

        public string Volume => @"\\?\Volume" + PartitionGuid.ToString("b") + @"\";

        protected bool Equals(Partition other)
        {
            return PartitionGuid.Equals(other.PartitionGuid);
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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Partition) obj);
        }

        public override int GetHashCode()
        {
            return PartitionGuid.GetHashCode();
        }
    }
}