using System.IO;
using ByteSizeLib;
using Deployer.FileSystem;
using Deployer.FileSystem.Gpt;
using Xunit;

namespace Deployer.Tests
{
    public class GptTransactionTest
    {
        [Fact]
        public void CreateLayout()
        {
            using (var gpt = new GptContext(3, FileAccess.ReadWrite))
            {
                gpt.RemoveExisting("EFIESP64");
                gpt.RemoveExisting("SYSTEM");
                gpt.RemoveExisting("MSR");
                gpt.RemoveExisting("Windows");
                gpt.RemoveExisting("Recovery");
            }

            using (var gpt = new GptContext(3, FileAccess.ReadWrite))
            {
                gpt.Add(new EntryBuilder("EFIESP64", ByteSize.FromMegaBytes(16), PartitionType.Basic)
                    .NoAutoMount()
                    .Build());

                gpt.Add(new EntryBuilder("SYSTEM", ByteSize.FromMegaBytes(100), PartitionType.Esp).NoAutoMount().Build());

                gpt.Add(new EntryBuilder("MSR", ByteSize.FromMegaBytes(16), PartitionType.Reserved).NoAutoMount().Build());

                var windowsSize = gpt.AvailableSize - ByteSize.FromMegaBytes(500);
                gpt.Add(new EntryBuilder("Windows", windowsSize, PartitionType.Basic).Build());

                gpt.Add(new EntryBuilder("Recovery", ByteSize.FromMegaBytes(500), PartitionType.Recovery)
                    .NoAutoMount()
                    .MarkAsCritical()
                    .Build());
                
            }
        }
    }    
}