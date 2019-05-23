using System;
using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Serilog;

namespace Deployer.FileSystem
{
    public static class FileSystemMixin
    {
        private static readonly ByteSize ValidResizeThreshold = ByteSize.FromMegaBytes(200);


        public static string CombineRelativeBcdPath(this string root)
        {
            return Path.Combine(root, "EFI", "Microsoft", "Boot", "BCD");
        }

        //public static async Task<IVolume> GetVolumeByPartitionName(this IDisk disk, string name)
        //{
        //    var partition = await disk.GetPartition(name);
        //    var vol = await partition.GetVolume();

        //    if (vol == null)
        //    {
        //        throw new ApplicationException("Cannot get the required IVolume");
        //    }

        //    await vol.Mount();

        //    return vol;
        //}

        //public static async Task<IVolume> GetOptionalVolumeByPartitionName(this IDisk disk, string name)
        //{
        //    var partition = await disk.GetOptionalPartition(name);

        //    if (partition == null)
        //    {
        //        return null;
        //    }

        //    var vol = await partition.GetVolume();

        //    if (vol != null)
        //    {
        //        await vol.Mount();

        //    }
            
        //    return vol;
        //}

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
    }
}