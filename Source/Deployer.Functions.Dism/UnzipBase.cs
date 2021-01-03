using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Serilog;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public abstract class UnzipBase : DeployerFunction
    {
        protected readonly IZipExtractor Extractor;
        private readonly IDownloader downloader;
        protected readonly IOperationProgress Progress;

        public UnzipBase(IZipExtractor extractor, IDownloader downloader, IOperationProgress progress,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.Extractor = extractor;
            this.downloader = downloader;
            this.Progress = progress;
        }

        public async Task Execute(string url, string destination, string artifactName = null)
        {
            var finalDir = Path.Combine(destination, artifactName ?? Path.GetFileNameWithoutExtension(GetFileName(url)));

            if (FileSystemOperations.DirectoryExists(finalDir))
            {
                Log.Warning("{Url} already downloaded. Skipping download.", url);
                return;
            }

            using (var stream = await GetStream(url))
            {
                await Extract(stream, finalDir);
            }
        }

        protected abstract Task Extract(Stream stream, string finalDir);

        private Task<Stream> GetStream(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return downloader.GetStream(url, Progress);
            }

            return Task.FromResult<Stream>(File.OpenRead(url));
        }

        private static string GetFileName(string urlString)
        {
            if (Uri.TryCreate(urlString, UriKind.Absolute, out var uri))
            {
                var filename = Path.GetFileName(uri.LocalPath);
                return filename;
            }

            if (File.Exists(urlString))
            {
                return Path.GetFileName(urlString);
            }

            throw new InvalidOperationException($"Unsupported URL: {urlString}");
        }
    }
}