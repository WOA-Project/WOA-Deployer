using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public class Entry
    {
        public Entry(string name, ByteSize size, PartitionType gptType)
        {
            Name = name;
            Size = size;
            GptType = gptType;
        }

        public string Name { get; }
        public ByteSize Size { get; }
        public PartitionType GptType { get; }
        public ulong Attributes { get; set; }
    }
}