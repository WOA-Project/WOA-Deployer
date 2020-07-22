using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class Unmount : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public Unmount(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(string partitionDescriptor)
        {
            var part = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            await part.DoAsync((p, token) => p.RemoveDriveLetter());
        }
    }
}