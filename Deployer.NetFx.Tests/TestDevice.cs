using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Zafiro.Core;

namespace Deployer.NetFx.Tests
{
    public class TestDevice : Device
    {
        private static readonly ByteSize MinimumPhoneDiskSize = ByteSize.FromGigaBytes(28);
        private static readonly ByteSize MaximumPhoneDiskSize = ByteSize.FromGigaBytes(34);

        public TestDevice(IDiskApi diskApi) : base(diskApi)
        {
        }

        public override async Task<Disk> GetDeviceDisk()
        {
            var disk = await GetDeviceDiskCore();
            if (disk.IsOffline)
            {
                throw new ApplicationException(
                    "The phone disk is offline. Please, set it online with Disk Management or DISKPART.");
            }

            return disk;
        }

        public override Task<Volume> GetWindowsVolume()
        {
            return this.GetOptionalVolumeByPartitionName(PartitionName.Windows);
        }

        protected override Task<bool> IsWoAPresent()
        {
            throw new NotImplementedException();
        }

        public override async Task<Volume> GetSystemVolume()
        {
            return await (await GetDeviceDisk()).GetVolumeByPartitionName(PartitionName.System);
        }

        public override async Task<Partition> GetSystemPartition()
        {
            var disk = await GetDeviceDisk();
            return await disk.GetOptionalPartition(PartitionName.System);
        }

        private async Task<Disk> GetDeviceDiskCore()
        {
            var disks = await DiskApi.GetDisks();

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

        private static async Task<bool> IsDeviceDisk(Disk disk)
        {
            var hasCorrectSize = HasCorrectSize(disk);

            if (!hasCorrectSize)
            {
                return false;
            }

            var diskNames = new[] { "VEN_QUALCOMM&PROD_MMC_STORAGE", "VEN_MSFT&PROD_PHONE_MMC_STOR" };
            var hasCorrectDiskName = diskNames.Any(name => disk.UniqueId.Contains((string) name));

            if (hasCorrectDiskName)
            {
                return true;
            }

            var partitions = await disk.GetPartitions();
            var names = partitions.Select(x => x.Name);
            var lookup = new[] { "EFIESP", "TZAPPS", "DPP" };

            return lookup.IsSubsetOf(names);
        }



        private static bool HasCorrectSize(Disk disk)
        {
            var moreThanMinimum = disk.Size > MinimumPhoneDiskSize;
            var lessThanMaximum = disk.Size < MaximumPhoneDiskSize;
            return moreThanMinimum && lessThanMaximum;
        }
    }
}