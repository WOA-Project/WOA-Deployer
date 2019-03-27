using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Deployer.FileSystem
{
    public static class FileSystemMixin
    {
        public static string CombineRelativeBcdPath(this string root)
        {
            return Path.Combine(root, "EFI", "Microsoft", "Boot", "BCD");
        }

        public static async Task<Volume> GetVolumeByLabel(this Disk disk, string label)
        {
            var parts = await disk.GetPartitions();

            var query = parts
                .ToObservable()
                .Select(partition => Observable.FromAsync(partition.GetVolume))
                .Merge(1)
                .Where(x => x != null)
                .FirstOrDefaultAsync(x => string.Equals(x.Label, label, StringComparison.InvariantCultureIgnoreCase));

            return await query;
        }

        public static async Task<Partition> GetPartitionByVolumeLabel(this Disk disk, string label)
        {
            var vol = await disk.GetVolumeByLabel(label);
            return vol.Partition;
        }

        public static async Task<Partition> GetPartitionByName(this Disk disk, string name)
        {
            var listsOfPartitions = disk.GetPartitions().ToObservable();

            var matching = from partitions in listsOfPartitions
                           from partition in partitions
                           where string.Equals(partition.Name, name, StringComparison.InvariantCultureIgnoreCase)
                           select partition;

            var found = await matching.FirstOrDefaultAsync();

            if (found == null)
            {
                throw new ApplicationException($"Cannot find partition named '{name}'");
            }

            return found;
        }

        public static async Task<Volume> GetVolumeByPartitionName(this Disk disk, string name)
        {
            var partition = await GetPartitionByName(disk, name);
            return await partition.GetVolume();
        }
    }
}