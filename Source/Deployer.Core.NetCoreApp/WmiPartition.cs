using System;
using ByteSizeLib;
using Deployer.Core.FileSystem;

namespace Deployer.Core.NetCoreApp
{
    internal class WmiPartition
    {
        public uint Number { get; set; }
        public string UniqueId { get; set; }
        public Guid? Guid { get; set; }
        public string Root { get; set; }
        public GptType GptType { get; set; }
        public ByteSize Size { get; set; }
    }
}