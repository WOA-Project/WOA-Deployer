using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.FileSystem;

namespace Deployer.Core
{
    public class TestVolume : IVolume
    {
        public TestVolume()
        {
            throw new NotImplementedException();
        }

        public Task Mount()
        {
            throw new NotImplementedException();
        }

        public string Root { get; }
        public string Label { get; }
        public FileSystemFormat FileSystemFormat { get; }
        public ByteSize Size { get; set; }
        public char? Letter { get; }
        public IPartition Partition { get; }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            throw new NotImplementedException();
        }

        public Task Format(FileSystemFormat fileSystemFormat, string label)
        {
            throw new NotImplementedException();
        }

        public Task EnsureWriteable()
        {
            throw new NotImplementedException();
        }
    }
}