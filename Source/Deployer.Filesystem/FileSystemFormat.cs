using System;
using System.Linq;

namespace Deployer.Filesystem
{
    public class FileSystemFormat
    {
        public string Moniker { get; }

        public static FileSystemFormat Ntfs = new FileSystemFormat("NTFS");
        public static FileSystemFormat Fat16 = new FileSystemFormat("FAT");
        public static FileSystemFormat Fat32 = new FileSystemFormat("FAT32");

        private FileSystemFormat(string moniker)
        {
            Moniker = moniker;
        }

        public override string ToString()
        {
            return Moniker;
        }

        public static FileSystemFormat FromString(string moniker)
        {
            var types = new[] { Ntfs, Fat16, Fat32 };
            var found = types.FirstOrDefault(x => x.Moniker.Equals(moniker, StringComparison.InvariantCultureIgnoreCase));

            if (found is null)
            {
                return new FileSystemFormat("Unknown");
            }

            return found;
        }

        protected bool Equals(FileSystemFormat other)
        {
            return string.Equals(Moniker, other.Moniker);
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

            return Equals((FileSystemFormat) obj);
        }

        public override int GetHashCode()
        {
            return Moniker.GetHashCode();
        }
    }
}