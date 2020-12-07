using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Scripting.Functions.Partitions;
using Deployer.Filesystem;
using Optional.Async.Extensions;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class SetGptType : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public SetGptType(IFileSystem fileSystem,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(string partitionDescriptor, string gptTypeString)
        {
            var partition = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            await partition.MatchSomeAsync(p => p.SetGptType(GptType.FromString(gptTypeString)));
        }
    }
}