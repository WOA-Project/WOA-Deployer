using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Deployer.FileSystem
{
    public static class FileSystemMixin2
    {
        public static string CombineRelativeBcdPath(this string root)
        {
            return Path.Combine(root, "EFI", "Microsoft", "Boot", "BCD");
        }

        public static async Task<Volume> GetVolumeByPartitionName(this Disk disk, string name)
        {
            var partition = await disk.GetRequiredPartition(name);
            var vol = await partition.GetVolume();

            if (vol == null)
            {
                throw new ApplicationException("Cannot get the required volume");
            }

            await vol.Mount();

            return vol;
        }        
    }
}