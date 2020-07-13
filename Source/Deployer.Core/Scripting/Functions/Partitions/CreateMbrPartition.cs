using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class CreateMbrPartition : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public CreateMbrPartition(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<string> Execute(int diskNumber, string partitionType, string sizeString = null)
        {
            var size = sizeString != null ? ByteSize.Parse(sizeString) : default;
            var disk = await fileSystem.GetDisk(diskNumber);
            var partition = await disk.CreateGptPartition(GptType.FromString(partitionType), size);

            var descriptor = FileSystemMixin.GetDescriptor(partition);
            return await descriptor;
        }
    }
}