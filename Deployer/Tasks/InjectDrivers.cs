using System.IO;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;

namespace Deployer.Tasks
{
    [TaskDescription("Injecting drivers")]
    public class InjectDrivers : DeploymentTask
    {
        private readonly string origin;
        private readonly IDeploymentContext context;
        private readonly IWindowsImageService imageService;
        private readonly IFileSystemOperations fileSystemOperations;

        public InjectDrivers(string origin, IDeploymentContext context, IWindowsImageService imageService,
            IDeploymentContext deploymentContext, IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(deploymentContext, fileSystemOperations, operationContext)
        {
            this.origin = origin;
            this.context = context;
            this.imageService = imageService;
            this.fileSystemOperations = fileSystemOperations;
        }

        protected override async Task ExecuteCore()
        {
            var windowsPartition = await context.Device.GetWindowsPartition();
            await windowsPartition.EnsureWritable();
            var injectedDrivers = await imageService.InjectDrivers(origin, windowsPartition.Root);

            var metadataPath = GetMetadataFilename();

            SaveMetadata(injectedDrivers, Path.Combine(AppPaths.Metadata, "Injected Drivers", metadataPath));
        }

        private string GetMetadataFilename()
        {
            string finalFilename;
            do
            {
                var fileName = Path.GetFileNameWithoutExtension(origin);
                finalFilename = fileName + "_" + Path.GetRandomFileName() + "Info.json";
            } while (fileSystemOperations.FileExists(finalFilename));

            return finalFilename;
        }
    }
}