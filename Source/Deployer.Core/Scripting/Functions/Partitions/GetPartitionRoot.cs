using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class GetPartitionRoot : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public GetPartitionRoot( IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext, IFileSystem fileSystem) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<string> Execute(string partitionDescriptor)
        {
            var part = await fileSystem.GetPartitionFromDescriptor(partitionDescriptor);
            await part.EnsureWritable();
            return part.Root.Remove(part.Root.Length - 1);
        }
    }
}