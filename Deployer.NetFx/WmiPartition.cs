using System;
using ByteSizeLib;
using Deployer.FileSystem;

namespace Deployer.NetFx
{
    internal class WmiPartition
    {
        public uint Number { get; set; }
        public string UniqueId { get; set; }
        public Guid Guid { get; set; }
        public string Root { get; set; }
        public PartitionType PartitionType { get; set; }
        public ByteSize Size { get; set; }
    }
}