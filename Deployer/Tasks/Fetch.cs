using System;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching zip from {0}")]
    public class Fetch : IDeploymentTask
    {
        private readonly string url;
        private readonly string destination;
        private readonly IZipExtractor extractor;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;

        public Fetch(string url, string destination, IZipExtractor extractor,
            IFileSystemOperations fileSystemOperations, IDownloader downloader, IOperationProgress progressObserver)
        {
            this.url = url;
            this.destination = destination;
            this.extractor = extractor;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                return;
            }

            var stream = await downloader.GetStream(url, progressObserver);
            await extractor.ExtractToFolder(stream, destination);
        }
    }
}