using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Exceptions;
using Deployer.Core.FileSystem;
using Zafiro.Core.Mixins;

namespace Deployer.Lumia
{
    public static class Detection
    {
        private static readonly ByteSize MinimumPhoneDiskSize = ByteSize.FromGigaBytes(28);
        private static readonly ByteSize MaximumPhoneDiskSize = ByteSize.FromGigaBytes(34);
        
        private static async Task<bool> IsDeviceDisk(IDisk disk)
        {
            var hasCorrectSize = HasCorrectSize(disk);

            if (!hasCorrectSize)
            {
                return false;
            }

            var diskNames = new[] { "VEN_QUALCOMM&PROD_MMC_STORAGE", "VEN_MSFT&PROD_PHONE_MMC_STOR" };
            var hasCorrectDiskName = diskNames.Any(name => disk.UniqueId.Contains(name));

            if (hasCorrectDiskName)
            {
                return true;
            }

            var partitions = await disk.GetPartitions();
            var names = partitions.Select(x => x.Name);
            var lookup = new[] { "EFIESP", "TZAPPS", "DPP" };

            return lookup.IsSubsetOf(names);
        }

        private static bool HasCorrectSize(IDisk disk)
        {
            var moreThanMinimum = disk.Size > MinimumPhoneDiskSize;
            var lessThanMaximum = disk.Size < MaximumPhoneDiskSize;
            return moreThanMinimum && lessThanMaximum;
        }


        public static async Task<IDisk> GetDisk(Core.FileSystem.IFileSystem fileSystem)
        {
            var disks = await fileSystem.GetDisks();

            var disk = await disks
                .ToObservable()
                .SelectMany(async x => new { IsDevice = await IsDeviceDisk(x), Disk = x })
                .Where(x => x.IsDevice)
                .Select(x => x.Disk)
                .FirstOrDefaultAsync();

            if (disk != null)
            {
                return disk;
            }

            throw new PhoneDiskNotFoundException(
                "Cannot get the Phone Disk. Please, verify that the Phone is in Mass Storage Mode.");
        }
    }
}