using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Deployer.FileSystem.Gpt;
using Xunit;

namespace Deployer.Tests.Real
{
    public class GptTransactionTest
    {
        [Fact(Skip = "Don't run this")]
        [Trait("Category", "Real")]
        public async Task CreateLayout()
        {
            using (var gpt = await GptContextFactory.Create(3, FileAccess.ReadWrite))
            {
                gpt.RemoveExisting("SYSTEM");
                gpt.RemoveExisting("MSR");
                gpt.RemoveExisting("Windows");
                gpt.RemoveExisting("Recovery");
            }

            using (var gpt = await GptContextFactory.Create(3, FileAccess.ReadWrite))
            {
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

        [Fact]
        public async Task CheckRecovery()
        {
            using (var gpt = await GptContextFactory.Create(3, FileAccess.ReadWrite))
            {
                var part = gpt.Get("Recovery");
            }
        }

    }       
}