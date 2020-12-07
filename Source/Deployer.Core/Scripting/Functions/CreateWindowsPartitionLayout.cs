using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;
using Deployer.Filesystem.Gpt;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class CreateWindowsPartitionLayout : DeployerFunction
    {
        private readonly IFileSystem fileSystem;
        private readonly ByteSize reservedSize = ByteSize.FromMegaBytes(16);
        private readonly ByteSize systemSize = ByteSize.FromMegaBytes(100);

        public CreateWindowsPartitionLayout(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(int diskNumber)
        {
            var disk = await fileSystem.GetDisk(diskNumber);

            Log.Verbose("Creating partitions");

            using (var t = await GptContextFactory.Create((uint) diskNumber, FileAccess.ReadWrite))
            {
                t.Add(new EntryBuilder("SYSTEM", systemSize, GptType.Esp)
                    .NoAutoMount()
                    .Build());

                t.Add(new EntryBuilder("MSR", reservedSize, GptType.Reserved)
                    .NoAutoMount()
                    .Build());

                var windowsSize = t.AvailableSize;
                t.Add(new EntryBuilder("Windows", windowsSize, GptType.Basic)
                    .NoAutoMount()
                    .Build());
            }

            await disk.Refresh();
        }
    }
}