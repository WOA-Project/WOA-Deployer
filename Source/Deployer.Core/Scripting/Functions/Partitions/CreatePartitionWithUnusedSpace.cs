using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class CreatePartitionUsingAvailableSpace : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public CreatePartitionUsingAvailableSpace(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<string> Execute(int diskNumber, string partitionType, string label = "")
        {
            var disk = await fileSystem.GetDisk(diskNumber);
            var partition = await disk.CreatePartition(PartitionType.FromString(partitionType), label);

            var descriptor = FileSystemMixin.GetDescriptor(partition);
            return await descriptor;
        }
    }
}