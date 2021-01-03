using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Serilog;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class Fetch : DeployerFunction
    {
        private readonly IDownloader downloader;
        private readonly IOperationProgress progress;

        public Fetch(IDownloader downloader, IOperationProgress progress,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.downloader = downloader;
            this.progress = progress;
        }

        public async Task Execute(string url, string destination)
        {
            if (FileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("{Url} already downloaded. Skipping download.", url);
                return;
            }

            FileSystemOperations.EnsureDirectoryExists(Path.GetDirectoryName(destination));
            using (var stream = await downloader.GetStream(url, progress))
            using (var file = FileSystemOperations.OpenForWrite(destination))
            {
                stream.Position = 0;
                await stream.CopyToAsync(file);
            }
        }
    }
}