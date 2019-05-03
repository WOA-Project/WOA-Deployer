using System;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Serilog;

namespace Deployer
{
    public static class DeviceMixin
    {
        private static readonly ByteSize ValidResizeThreshold = ByteSize.FromMegaBytes(200);
      
        public static async Task<bool> HasEnoughSpace(this IDevice device, ByteSize requiredSize)
        {
            return (await device.GetDeviceDisk()).HasEnoughSpace(requiredSize);
        }

        public static bool HasEnoughSpace(this Disk disk, ByteSize requiredSize)
        {
            Log.Verbose("Available {Available}. Required {Required}", disk.AvailableSize, requiredSize);

            if (disk.AvailableSize >= requiredSize)
            {                
                Log.Verbose("We have enough available space!");
                return true;
            }

            Log.Verbose("We don't have enough space. Checking for tolerable threshold of {Threshold}", ValidResizeThreshold);

            var diff = disk.AvailableSize - requiredSize;
            var isThereEnoughSpace = Math.Abs(diff.MegaBytes) <= ValidResizeThreshold.MegaBytes;

            Log.Verbose("Available - Required => {Available} - {Required} = {Difference}",disk.AvailableSize, requiredSize, diff);
            Log.Verbose("Enough space? {Result}", isThereEnoughSpace);

            return isThereEnoughSpace;
        }

        public static async Task<Volume> GetVolumeByPartitionName(this IDevice device, string partitionName)
        {
            var disk = await device.GetDeviceDisk();
            return await disk.GetVolumeByPartitionName(partitionName);
        }
    }
}