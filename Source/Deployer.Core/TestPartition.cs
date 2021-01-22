using System;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Filesystem;

namespace Deployer.Core
{
    public class TestPartition : IPartition
    {
        public TestPartition(IDisk disk)
        {
        }

        public Task Format(FileSystemFormat fileSystemFormat, string label)
        {
            throw new NotImplementedException();
        }

        public IDisk Disk { get; }
        public string GptName { get; set; }
        public GptType GptType { get; set; }
        public char? Letter { get; }
        public string Root { get; set; }
        public Guid? Guid { get; set; }
        public uint Number { get; set; }

        public Task Remove()
        {
            throw new NotImplementedException();
        }

        public Task<IVolume> GetVolume()
        {
            throw new NotImplementedException();
        }

        public Task SetGptType(GptType gptType)
        {
            throw new NotImplementedException();
        }

        public Task<char> AssignDriveLetter()
        {
            throw new NotImplementedException();
        }

        public ByteSize Size { get; set; }
        public Task Resize(ByteSize size)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDriveLetter()
        {
            throw new NotImplementedException();
        }
    }
}