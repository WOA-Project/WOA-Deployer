using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting.Functions.Partitions;

namespace Deployer.Core.FileSystem
{
    public interface IDisk
    {
        Task Refresh();
        uint Number { get; }
        string FriendlyName { get; }
        ByteSize Size { get; }
        bool IsOffline { get; }
        string UniqueId { get; }
        ByteSize AvailableSize { get; }
        ByteSize AllocatedSize { get; }
        Task<IPartition> CreatePartition(ByteSize desiredSize, PartitionType partitionType, string name);
        Task<IList<IPartition>> GetPartitions();
        Task SetGuid(Guid guid);
        Task ToggleOnline(bool b);
        Task PrepareForRemoval();
        Task ClearAs(DiskType mbr);
    }
}