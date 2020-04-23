using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.Core.FileSystem
{
    public static class DiskMixin
    {
        public static Task<IPartition> CreatePartition(this IDisk self, PartitionType partitionType, string label = "")
        {
            return self.CreatePartition(ByteSize.MaxValue, partitionType, label);
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
            return partitions.OrderBy(x => x.Number).ToList()[number - 1];
        }

        public static async Task<IPartition> GetPartitionByName(this IDisk disk, string name)
        {
            var partitions = await disk.GetPartitions();
            var matching = from p in partitions
                where string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase)
                select p;

            return matching.FirstOrDefault();
        }
    }
}