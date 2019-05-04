using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;

namespace Deployer
{
    public static class DeviceMixin
    {
        public static async Task<bool> HasEnoughSpace(this IDevice device, ByteSize requiredSize)
        {
            return (await device.GetDeviceDisk()).HasEnoughSpace(requiredSize);
        }
        
        public static async Task<Volume> GetVolumeByPartitionName(this IDevice device, string partitionName)
        {
            var disk = await device.GetDeviceDisk();
            return await disk.GetVolumeByPartitionName(partitionName);
        }
    }
}