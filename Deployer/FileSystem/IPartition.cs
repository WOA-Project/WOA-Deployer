using System;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.FileSystem
{
    public interface IPartition
    {
        IDisk Disk { get; }
        string Name { get; set; }
        PartitionType PartitionType { get; set; }
        string Root { get; set; }
        Guid Guid { get; set; }
        uint Number { get; set; }
        Task<IVolume> GetVolume();
        Task SetGptType(PartitionType partitionType);
        Task<char> AssignDriveLetter();
        ByteSize Size { get; set; }
        Task Resize(ByteSize size);
    }
}