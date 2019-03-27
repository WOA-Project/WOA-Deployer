using System.Collections.ObjectModel;
using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public interface IGptContext
    {
        ByteSize AvailableSize { get; }
        ReadOnlyCollection<Partition> Partitions { get; }
        ByteSize AllocatedSize { get; }
        ByteSize TotalSize { get; }
        void Add(Entry entry);
        void Delete(Partition partition);
    }
}