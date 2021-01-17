using System;

namespace Deployer.Filesystem
{
    public class DiskType
    {
        public static DiskType Raw = new DiskType("RAW");
        public static DiskType Mbr = new DiskType("MBR");
        public static DiskType Gpt = new DiskType("GPT");
        public static DiskType Unsupported = new DiskType("Unsupported");

        private DiskType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static DiskType FromString(string str)
        {
            switch (str.ToUpper())
            {
                case "RAW":
                    return Raw;
                case "MBR":
                    return Mbr;
                case "GPT":
                    return Gpt;
                default:
                    return Unsupported;
            }
        }

        protected bool Equals(DiskType other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiskType) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}