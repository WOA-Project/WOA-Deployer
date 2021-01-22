using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Filesystem.Gpt;

namespace Deployer.Filesystem
{
    public static class DiskMixin
    {
        public static Task<IPartition> CreatePartition(this IDisk self, GptType gptType, string label = "")
        {
            return self.CreateGptPartition(gptType, ByteSize.MaxValue);
        }

        public static async Task<IList<IVolume>> GetVolumes(this IDisk self)
        {
            var partitions = await self.GetPartitions();

            return await partitions
                .ToObservable()
                .Select(x => Observable.FromAsync(x.GetVolume))
                .Merge(1)
                .Where(x => x != null)
                .ToList();
        }

        public static async Task<IVolume> GetVolume(this IDisk self, string label)
        {
            var volumes = await self.GetVolumes();
            return volumes.FirstOrDefault(x => string.Equals(x.Label, label));
        }

        public static async Task<IPartition> GetPartitionByVolumeLabel(this IDisk disk, string label)
        {
            var volumes = await disk.GetVolumes();
            var matching = from v in volumes
                where string.Equals(v.Label, label, StringComparison.InvariantCultureIgnoreCase)
                select v.Partition;

            return matching.FirstOrDefault();
        }

        public static async Task<IPartition> GetPartitionByNumber(this IDisk disk, int number)
        {
            var partitions = await disk.GetPartitions();
            return partitions.FirstOrDefault(x => x.Number == number);
        }

        public static async Task<IPartition> GetPartitionByName(this IDisk disk, string name)
        {
            var partitions = await disk.GetPartitions();
            var matching = from p in partitions
                where string.Equals(p.GptName, name, StringComparison.InvariantCultureIgnoreCase)
                select p;

            return matching.FirstOrDefault();
        }

        public static void ConfigureAsSuperFloppy(IDisk disk)
        {
            using (var deviceStream = new DeviceStream("\\\\.\\PhysicalDrive" + disk.Number, FileAccess.ReadWrite))
            {
                var buffer1 = new byte[87];
                deviceStream.Seek(0x100000, SeekOrigin.Begin);
                deviceStream.Read(buffer1, 0, buffer1.Length);
                Buffer.BlockCopy(BitConverter.GetBytes((short)(BitConverter.ToInt16(new[]
                {
                    buffer1[14],
                    buffer1[15]
                }, 0) + 2048)), 0, buffer1, 14, 2);
                var buffer2 = new byte[512];
                deviceStream.Seek(0L, SeekOrigin.Begin);
                deviceStream.Read(buffer2, 0, buffer2.Length);
                Buffer.BlockCopy(buffer1, 11, buffer2, 11, 76);
                deviceStream.Seek(0L, SeekOrigin.Begin);
                deviceStream.Write(buffer2, 0, buffer2.Length);
            }
        }
    }
}