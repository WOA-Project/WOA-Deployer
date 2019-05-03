using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub subfolder: {0}/{1} to {2}")]
    public class FetchGitHubFolder : IDeploymentTask
    {
        private readonly string url;
        private readonly string relativePath;
        private readonly string destination;
        private readonly IZipExtractor zipExtractor;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;

        public FetchGitHubFolder(string url, string relativePath, string destination, IZipExtractor zipExtractor,
            IFileSystemOperations fileSystemOperations, IDownloader downloader, IOperationProgress progressObserver)
        {
            this.url = url;
            this.relativePath = relativePath;
            this.destination = destination;
            this.zipExtractor = zipExtractor;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("{Url}{Folder} was already downloaded. Skipping download.", url, relativePath);
                return;
            }

            using (var stream = await GitHubMixin.GetBranchZippedStream(downloader, url, progressObserver: progressObserver))
            {
                await zipExtractor.ExtractRelativeFolder(stream, relativePath, destination);
            }
        }
    }
}