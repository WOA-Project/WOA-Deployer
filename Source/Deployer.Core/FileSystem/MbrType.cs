namespace Deployer.Core.FileSystem
{
    public class MbrType
    {
        public string Code { get; }
        public string Name { get; }

        public static readonly MbrType Fat12 = new MbrType(nameof(Fat12), "FAT12");
        public static readonly MbrType Fat16 = new MbrType(nameof(Fat16), "FAT16");
        public static readonly MbrType Extended = new MbrType(nameof(Extended), "Extended");
        public static readonly MbrType Huge = new MbrType(nameof(Huge) , "Huge");
        public static readonly MbrType Ifs = new MbrType(nameof(Ifs) , "IFS");
        public static readonly MbrType Fat32 = new MbrType(nameof(Fat32) , "FAT32");

        private MbrType(string code, string name)
        {
            Code = code;
            Name = name;
        }

        protected bool Equals(GptType other)
        {
            return Code.Equals(other.Code);
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
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}