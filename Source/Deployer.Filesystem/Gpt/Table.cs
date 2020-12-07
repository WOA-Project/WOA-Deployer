// Copyright (c) 2018, Rene Lergner - wpinternals.net - @Heathcliff74xda
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Deployer.Filesystem.Gpt
{
    internal class Table
    {
        private readonly uint headerOffset;
        private readonly uint headerSize;
        private readonly uint partitionEntrySize;

        public readonly List<Partition> Partitions = new List<Partition>();
        private byte[] gptBuffer;
        private uint tableOffset;
        private uint tableSize;

        internal Table(byte[] gptBuffer, uint bytesPerSector)
        {
            this.gptBuffer = gptBuffer;
            var tempHeaderOffset = ByteOperations.FindAscii(gptBuffer, "EFI PART");
            if (tempHeaderOffset == null)
            {
                throw new Exception("Bad GPT");
            }

            headerOffset = (uint) tempHeaderOffset;
            headerSize = ByteOperations.ReadUInt32(gptBuffer, headerOffset + 0x0C);
            tableOffset = headerOffset + 0x200;
            FirstUsableSector = ByteOperations.ReadUInt64(gptBuffer, headerOffset + 0x28);
            LastUsableSector = ByteOperations.ReadUInt64(gptBuffer, headerOffset + 0x30);
            var maxPartitions = ByteOperations.ReadUInt32(gptBuffer, headerOffset + 0x50);
            partitionEntrySize = ByteOperations.ReadUInt32(gptBuffer, headerOffset + 0x54);
            tableSize = maxPartitions * partitionEntrySize;
            if (tableOffset + tableSize > gptBuffer.Length)
            {
                throw new Exception("Bad GPT");
            }

            var partitionOffset = tableOffset;

            uint number = 1;
            while (partitionOffset < tableOffset + tableSize)
            {
                var name = ByteOperations.ReadUnicodeString(gptBuffer, partitionOffset + 0x38, 0x48)
                    .TrimEnd((char) 0, ' ');
                if (name.Length == 0)
                {
                    break;
                }

                var partitionTypeGuid = ByteOperations.ReadGuid(gptBuffer, partitionOffset + 0x00);
                var partitionType = GptType.FromGuid(partitionTypeGuid);

                var currentPartition = new Partition(name, partitionType, bytesPerSector)
                {
                    FirstSector = ByteOperations.ReadUInt64(gptBuffer, partitionOffset + 0x20),
                    LastSector = ByteOperations.ReadUInt64(gptBuffer, partitionOffset + 0x28),
                    Guid = ByteOperations.ReadGuid(gptBuffer, partitionOffset + 0x10),
                    Attributes = ByteOperations.ReadUInt64(gptBuffer, partitionOffset + 0x30)
                };
                Partitions.Add(currentPartition);
                partitionOffset += partitionEntrySize;
                number++;
            }
        }

        public ulong FirstUsableSector { get; }
        public ulong LastUsableSector { get; }

        internal Partition GetPartition(string name)
        {
            return Partitions.FirstOrDefault(p =>
                string.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        internal byte[] Rebuild()
        {
            Log.Debug("Rebuilding GPT...");

            if (gptBuffer == null)
            {
                tableSize = 0x4200;
                tableOffset = 0;
                gptBuffer = new byte[tableSize];
            }
            else
            {
                Array.Clear(gptBuffer, (int) tableOffset, (int) tableSize);
            }

            var partitionOffset = tableOffset;
            foreach (var currentPartition in Partitions)
            {
                ByteOperations.WriteGuid(gptBuffer, partitionOffset + 0x00, currentPartition.GptType.Guid);
                ByteOperations.WriteGuid(gptBuffer, partitionOffset + 0x10, currentPartition.Guid);
                ByteOperations.WriteUInt64(gptBuffer, partitionOffset + 0x20, currentPartition.FirstSector);
                ByteOperations.WriteUInt64(gptBuffer, partitionOffset + 0x28, currentPartition.LastSector);
                ByteOperations.WriteUInt64(gptBuffer, partitionOffset + 0x30, currentPartition.Attributes);
                ByteOperations.WriteUnicodeString(gptBuffer, partitionOffset + 0x38, currentPartition.Name, 0x48);

                partitionOffset += partitionEntrySize;
            }

            ByteOperations.WriteUInt32(gptBuffer, headerOffset + 0x58,
                ByteOperations.Crc32(gptBuffer, tableOffset, tableSize));
            ByteOperations.WriteUInt32(gptBuffer, headerOffset + 0x10, 0);
            ByteOperations.WriteUInt32(gptBuffer, headerOffset + 0x10,
                ByteOperations.Crc32(gptBuffer, headerOffset, headerSize));

            Log.Debug("GPT rebuilt");

            return gptBuffer;
        }
    }
}