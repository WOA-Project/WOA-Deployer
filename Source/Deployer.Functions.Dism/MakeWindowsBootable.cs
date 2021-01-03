using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Deployer.Tools.Bcd;
using Deployer.Tools.Dism;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class MakeWindowsBootable : DeployerFunction
    {
        private readonly IFileSystem fileSystem;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IBootCreator bootCreator;
        private readonly IWindowsImageService windowsImageService;
        private readonly IOperationProgress progress;

        public MakeWindowsBootable(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
            IBootCreator bootCreator,
             IOperationContext operationContext, IWindowsImageService windowsImageService, IOperationProgress progress) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
            this.fileSystemOperations = fileSystemOperations;
            this.bootCreator = bootCreator;
            this.windowsImageService = windowsImageService;
            this.progress = progress;
        }

        public Task Execute(string systemRoot, string windowsRoot)
        {
            return bootCreator.MakeBootable(systemRoot, windowsRoot);
        }
    }
}