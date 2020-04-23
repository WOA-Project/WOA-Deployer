using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class Unzip : DeployerFunction
    {
        private readonly IZipExtractor extractor;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progress;

        public Unzip(IZipExtractor extractor, IDownloader downloader, IOperationProgress progress,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.extractor = extractor;
            this.downloader = downloader;
            this.progress = progress;
        }

        public async Task Execute(string url, string destination, string artifactName = null)
        {
            var finalDir = Path.Combine(destination, artifactName ?? Path.GetFileNameWithoutExtension(GetFileName(url)));

            if (FileSystemOperations.DirectoryExists(finalDir))
            {
                Log.Warning("{Url} already downloaded. Skipping download.", url);
                return;
            }

            using (var stream = await downloader.GetStream(url, progress))
            {
                await extractor.ExtractFirstChildToFolder(stream, finalDir, progress);
            }
        }

        private static string GetFileName(string urlString)
        {
            var uri = new Uri(urlString);
            var filename = Path.GetFileName(uri.LocalPath);
            return filename;
        }
    }
}