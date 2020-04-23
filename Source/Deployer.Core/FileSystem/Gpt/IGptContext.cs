using System.Collections.ObjectModel;
using ByteSizeLib;

namespace Deployer.Core.FileSystem.Gpt
{
    public interface IGptContext
    {
        ByteSize AvailableSize { get; }
        ReadOnlyCollection<Partition> Partitions { get; }
        ByteSize AllocatedSize { get; }
        ByteSize TotalSize { get; }
        Partition Add(Entry entry);
        void Delete(Partition partition);
    }
}