using System;
using ByteSizeLib;

namespace Deployer.FileSystem.Gpt
{
    public class EntryBuilder
    {
        private readonly string name;
        private readonly ByteSize size;
        private readonly PartitionType gptType;

        public EntryBuilder(string name, ByteSize size, PartitionType gptType)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.size = size;
            this.gptType = gptType;
        }

        private bool markAsCritial;
        private bool noAutoMount;

        public EntryBuilder MarkAsCritical()
        {
            markAsCritial = true;
            return this;
        }

        public EntryBuilder NoAutoMount()
        {
            noAutoMount = true;
            return this;
        }

        public Entry Build()
        {
            ulong attributes = 0;
            if (noAutoMount)
            {
                attributes |= 0x8000000000000000;
            }

            if (markAsCritial)
            {
                attributes |= 0x0000000000000001;
            }
            
            return new Entry(name, size, gptType)
            {
                Attributes = attributes,
            };

        }
    }
}