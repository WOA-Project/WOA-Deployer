/*

Copyright (c) 2019, Gustave Monce - gus33000.me - @gus33000
Copyright (c) 2018, Proto Beta Test - protobetatest.com - @ProtoBetaTest

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System.Collections.Generic;
using System.IO;
using Serilog;
using Zafiro.Core.Mixins;

namespace Deployer.Filesystem.Gpt
{
    internal class Handler
    {
        private readonly Stream deviceStream;
        private readonly Table table;
        private readonly byte[] gptBuffer;
        private readonly int chunkSize = 0x20000;

        public List<Partition> Partitions => table.Partitions;
        public ulong Length => (ulong) deviceStream.Length;

        public Handler(Stream diskDevice, uint bytesPerSector)
        {
            // First initialize the stream
            deviceStream = diskDevice;

            // Create a buffer for the first chunk of the device
            gptBuffer = new byte[chunkSize];

            // Read the first chunk of the device
            deviceStream.Seek(0, SeekOrigin.Begin);
            deviceStream.Read(gptBuffer, 0, chunkSize);

            // Initialize a new GPT object
            table = new Table(gptBuffer, bytesPerSector);
        }

        public Partition GetPartition(string name) => table.GetPartition(name);

        public void Commit()
        {
            Log.Debug("About to commit this partition layout: \n{Layout}", Partitions.AsNumberedList());

            table.Rebuild();

            // Write back the first chunk of the device
            deviceStream.Seek(0, SeekOrigin.Begin);
            deviceStream.Write(gptBuffer, 0, chunkSize);

            Log.Debug("GPT changes committed successfully");
        }       
    }
}
