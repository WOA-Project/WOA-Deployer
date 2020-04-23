using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class Flash : DeployerFunction
    {
        private readonly IFileSystem fileSystem;
        private readonly IImageFlasher flasher;
        private readonly IOperationProgress progress;

        public Flash(IFileSystem fileSystem, IImageFlasher flasher, 
            IOperationContext operationContext, IFileSystemOperations fileSystemOperations, IOperationProgress progress) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
            this.flasher = flasher;
            this.progress = progress;
        }

        public async Task Execute(string imagePath, int diskNumber)
        {
            var disk = await fileSystem.GetDisk(diskNumber);
            await flasher.Flash(disk, imagePath, progress);
        }
    }
}