using System.Collections.Generic;
using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public interface IGptContext
    {
        ByteSize AvailableSize { get; }
        IEnumerable<Partition> Partitions { get; }
        ByteSize AllocatedSize { get; }
        ByteSize TotalSize { get; }
        void Add(Entry entry);
        void Delete(Partition partition);
    }
}