using System;
using ByteSizeLib;
using Deployer.Filesystem;

namespace Deployer.NetFx
{
    public class PartitionData
    {
        public string Root { get; set; }
        public uint Number { get; set; }
        public Guid? Guid { get; set; }
        public GptType GptType { get; set; }
        public string UniqueId { get; set; }
        public ByteSize Size { get; set; }
        public string Name { get; set; }
    }
}