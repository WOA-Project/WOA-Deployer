using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Services;
using Deployer.Tools.Dism;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions
{
    public class InjectDrivers : DeployerFunction
    {
        private readonly IWindowsImageService imageService;
        private readonly IFileSystemOperations fileSystemOperations;
        private string origin;

        public InjectDrivers(IWindowsImageService imageService,
             IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.imageService = imageService;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute(string windowsPath, string origin)
        {
            this.origin = origin;
            var injectedDrivers = await imageService.InjectDrivers(origin, windowsPath);
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