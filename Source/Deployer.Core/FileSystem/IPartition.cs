using System;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer.Core.FileSystem
{
    public interface IPartition
    {
        IDisk Disk { get; }
        string Name { get; set; }
        GptType GptType { get; set; }
        string Root { get; set; }
        Guid? Guid { get; set; }
        uint Number { get; set; }
        Task<IVolume> GetVolume();
        Task SetGptType(GptType gptType);
        Task<char> AssignDriveLetter();
        ByteSize Size { get; set; }
        Task Resize(ByteSize size);
        Task RemoveDriveLetter();
    }
}