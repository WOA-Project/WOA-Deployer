using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Optional.Async.Extensions;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

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