using System;
using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Serilog;

namespace Deployer.Filesystem
{
    public static class FileSystemMixin
    {
        private static readonly ByteSize ValidResizeThreshold = ByteSize.FromMegaBytes(200);


        public static string CombineRelativeBcdPath(this string root)
        {
            return Path.Combine(root, "EFI", "Microsoft", "Boot", "BCD");
        }

        public static bool HasEnoughSpace(this IDisk disk, ByteSize requiredSize)
        {
            Log.Verbose("Available {Available}. Required {Required}", disk.AvailableSize, requiredSize);

            if (disk.AvailableSize >= requiredSize)
            {
                Log.Verbose("We have enough available space!");
                return true;
            }

            Log.Verbose("We don't have enough space. Checking for tolerable threshold of {Threshold}",
                ValidResizeThreshold);

            var diff = disk.AvailableSize - requiredSize;
            var isThereEnoughSpace = Math.Abs(diff.MegaBytes) <= ValidResizeThreshold.MegaBytes;

            Log.Verbose("Available - Required => {Available} - {Required} = {Difference}", disk.AvailableSize,
                requiredSize, diff);
            Log.Verbose("Enough space? {Result}", isThereEnoughSpace);

            return isThereEnoughSpace;
        }

        public static async Task<string> GetDescriptor(IPartition partition)
        {
            var volume = await partition.GetVolume();
            return $"Disk={partition.Disk.Number}, Name='{partition?.GptName}', Label='{volume?.Label}', Number={partition.Number}";
        }
    }
}