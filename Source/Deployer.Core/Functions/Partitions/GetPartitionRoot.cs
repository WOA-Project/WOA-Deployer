using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;
using Optional.Async.Extensions;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Partitions
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
            var part = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            var root = await part.MapAsync(async p =>
            {
                await p.EnsureWritable();
                return p.Root.Remove(p.Root.Length - 1);
            });

            return root.ValueOr("");
        }
    }
}