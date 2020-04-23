using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
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
            var partition = await fileSystem.GetPartitionFromDescriptor(partitionDescriptor);
            var gptType = PartitionType.FromString(gptTypeString);
            await partition.SetGptType(gptType);
        }
    }
}