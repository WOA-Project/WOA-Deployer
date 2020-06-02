using ByteSizeLib;

namespace Deployer.Core.FileSystem.Gpt
{
    public class Entry
    {
        public Entry(string name, ByteSize size, GptType gptType)
        {
            Name = name;
            Size = size;
            GptType = gptType;
        }

        public string Name { get; }
        public ByteSize Size { get; }
        public GptType GptType { get; }
        public ulong Attributes { get; set; }
    }
}