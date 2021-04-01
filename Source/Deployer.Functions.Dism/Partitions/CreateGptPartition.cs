using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Partitions
{
    public class CreateGptPartition : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public CreateGptPartition(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<string> Execute(int diskNumber, string partitionType, string gptName = "", string sizeString = null)
        {
            var size = sizeString != null ? ByteSize.Parse(sizeString) : default;
            var disk = await fileSystem.GetDisk(diskNumber);
            var partition = await disk.CreateGptPartition(GptType.FromString(partitionType), gptName, size);

            var descriptor = FileSystemMixin.GetDescriptor(partition);
            return await descriptor;
        }
    }
}