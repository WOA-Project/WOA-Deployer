using System;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.FileSystem;

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
        public string Name { get; set; }
        public PartitionType PartitionType { get; set; }
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

        public Task SetGptType(PartitionType partitionType)
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