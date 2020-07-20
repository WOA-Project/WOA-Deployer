using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class ApplyWindowsImage : DeployerFunction
    {
        private readonly IWindowsImageService windowsImageService;
        private readonly IOperationProgress progress;

        public ApplyWindowsImage(IFileSystemOperations fileSystemOperations, IOperationContext operationContext,
            IWindowsImageService windowsImageService, IOperationProgress progress) : base(fileSystemOperations,
            operationContext)
        {
            this.windowsImageService = windowsImageService;
            this.progress = progress;
        }

        public Task Execute(string wimFile, int index, string path)
        {
            return windowsImageService.ApplyImage(path, wimFile, index, progressObserver: progress);
        }
    }
}