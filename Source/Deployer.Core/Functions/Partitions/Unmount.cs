using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;
using Optional.Async.Extensions;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Partitions
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
            await part.MatchSomeAsync(p => p.RemoveDriveLetter());
        }
    }
}