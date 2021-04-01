using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Partitions
{
    public class PartitionExists : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public PartitionExists(IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext, IFileSystem fileSystem) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<bool> Execute(string partitionDescriptor)
        {
            var part = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            return part.HasValue;
        }
    }
}