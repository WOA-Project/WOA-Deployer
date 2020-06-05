using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class RemovePartition : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public RemovePartition(IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext, IFileSystem fileSystem) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(string partitionDescriptor)
        {
            var partition = await fileSystem.GetPartitionFromDescriptor(partitionDescriptor);
            await partition.Remove();
            await partition.Disk.Refresh();
        }
    }
}