using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Functions.Partitions;

namespace Deployer.Core
{
    public class TestDisk : IDisk
    {
        public Task Refresh()
        {
            throw new NotImplementedException();
        }

        public uint Number { get; set; }
        public string FriendlyName { get; }
        public ByteSize Size { get; }
        public bool IsOffline { get; set; }
        public string UniqueId { get; set; }
        public ByteSize AvailableSize { get; }
        public ByteSize AllocatedSize { get; }

        public Task<IPartition> CreateMbrPartition(MbrType mbrType, ByteSize size = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPartition> CreateGptPartition(GptType gptType, ByteSize desiredSize)
        {
            throw new NotImplementedException();
        }

        public Task<IList<IPartition>> GetPartitions()
        {
            throw new NotImplementedException();
        }

        public Task SetGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Task ToggleOnline(bool b)
        {
            throw new NotImplementedException();
        }

        public Task PrepareForRemoval()
        {
            throw new NotImplementedException();
        }

        public Task ClearAs(DiskType diskType)
        {
            throw new NotImplementedException();
        }
    }
}