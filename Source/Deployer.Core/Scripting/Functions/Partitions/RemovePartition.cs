using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Optional.Async.Extensions;
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
            var partition = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            await partition.MatchSomeAsync(async p =>
            {
                await p.Remove();
                await p.Disk.Refresh();
            });
        }
    }
}