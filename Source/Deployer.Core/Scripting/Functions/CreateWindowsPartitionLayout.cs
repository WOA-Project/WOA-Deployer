using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.FileSystem;
using Deployer.Core.FileSystem.Gpt;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
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
             IOperationContext operationContext, IWindowsImageService windowsImageService, IOperationProgress progress) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(int diskNumber)
        {
            var disk = await fileSystem.GetDisk(diskNumber);

            Log.Verbose("Creating partitions");

            using (var t = await GptContextFactory.Create((uint) diskNumber, FileAccess.ReadWrite))
            {
                t.Add(new EntryBuilder("SYSTEM", systemSize, PartitionType.Esp)
                    .NoAutoMount()
                    .Build());

                t.Add(new EntryBuilder("MSR", reservedSize, PartitionType.Reserved)
                    .NoAutoMount()
                    .Build());

                var windowsSize = t.AvailableSize;
                t.Add(new EntryBuilder("Windows", windowsSize, PartitionType.Basic)
                    .NoAutoMount()
                    .Build());
            }

            await disk.Refresh();
        }
    }
}