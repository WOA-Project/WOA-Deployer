using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.FileSystem.Gpt;

namespace Deployer.FileSystem
{
    public static class DiskMixin
    {
        public static async Task<Partition> GetPartition(this Disk self,string name)
        {
            return await Observable.FromAsync(async () =>
            {
                using (var context = await GptContextFactory.Create(self.Number, FileAccess.Read))
                {
                    var partition = context.Partitions.FirstOrDefault(x =>
                        string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));

                    if (partition == null)
                    {
                        throw new ApplicationException($"Cannot find partition named {name} in {self}");
                    }

                    return partition.AsCommon(self);
                }
            }).RetryWithBackoffStrategy();
        }

        public static async Task<Partition> GetOptionalPartition(this Disk self, string name)
        {
            var partitions = await self.GetPartitions();
            return partitions.FirstOrDefault(x =>
                string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}