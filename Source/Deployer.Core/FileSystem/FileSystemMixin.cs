using System;
using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting.MicroParser;
using Serilog;

namespace Deployer.Core.FileSystem
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
            return $"Disk={partition.Disk.Number}, Name='{partition?.Name}', Label='{volume?.Label}', Number={partition.Number}";
        }

        public static async Task<IPartition> GetPartitionFromDescriptor(this IFileSystem fileSystem, string descriptor)
        {
            var mini = Parser.Parse(descriptor);
            var diskNumber = (int?)mini["Disk"];

            if (diskNumber == null)
            {
                throw new InvalidOperationException($"Disk value is missing while parsing partition descriptor: {descriptor}");
            }

            var label = (string)mini["Label"];
            var name = (string)mini["Name"];
            var number = (int?)mini["Number"];

            var disk = await fileSystem.GetDisk(diskNumber.Value);
            IPartition part = null;

            if (number.HasValue)
            {
                part = await disk.GetPartitionByNumber(number.Value);
            }

            if (part is null && !(label is null))
            {
                part = await disk.GetPartitionByVolumeLabel(label);
            }

            if (part is null && name != null)
            {
                part = await disk.GetPartitionByName(name);
            }

            if (part == null)
            {
                throw new FileSystemException($"Cannot find partition from descriptor {descriptor}");
            }

            return part;
        }
    }
}