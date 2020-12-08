using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;
using Deployer.Functions.Partitions;
using Optional.Async.Extensions;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
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