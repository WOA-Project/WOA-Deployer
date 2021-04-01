using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class GetDiskSize : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public GetDiskSize(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<string> Execute(int diskNumber)
        {
            return (await fileSystem.GetDisk(diskNumber))
                .AvailableSize.ToString();
        }
    }
}