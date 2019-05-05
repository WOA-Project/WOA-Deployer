using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem.Gpt;

namespace Deployer.FileSystem
{
    public class Disk
    {
        public Disk(IDiskApi diskApi, DiskInfo diskProps)
        {
            DiskApi = diskApi;
            FriendlyName = diskProps.FriendlyName;
            Number = diskProps.Number;
            Size = diskProps.Size;
            AllocatedSize = diskProps.AllocatedSize;
            FriendlyName = diskProps.FriendlyName;
            IsSystem = diskProps.IsSystem;
            IsBoot = diskProps.IsBoot;
            IsReadOnly = diskProps.IsReadOnly;
            IsOffline = diskProps.IsOffline;
            UniqueId = diskProps.UniqueId;
        }

        public ByteSize Size { get; }

        public bool IsBoot { get; }

        public bool IsReadOnly { get; }

        public bool IsOffline { get; }

        public bool IsSystem { get; }

        public ByteSize AllocatedSize { get; }

        public string FriendlyName { get; }

        public IDiskApi DiskApi { get; }

        public uint Number { get; }
        public ByteSize AvailableSize => Size - AllocatedSize;
        public string UniqueId { get; }

        public async Task<List<Partition>> GetPartitions()
        {
            using (var context = await GptContextFactory.Create(Number, FileAccess.Read))
            {
                return context.Partitions.Select(x => x.AsCommon(this)).ToList();
            }
        }
        
        public Task Refresh()
        {
            return DiskApi.RefreshDisk(this);
        }

        public Task SetGuid(Guid guid)
        {
            return DiskApi.ChangeDiskId(this, guid);
        }

        public override string ToString()
        {
            return $"Disk {Number} ({FriendlyName})";
        }
    }

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