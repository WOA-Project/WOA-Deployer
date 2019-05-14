using System;
using System.Threading;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching zip from {0}")]
    public class Fetch : DeploymentTask
    {
        private readonly string url;
        private readonly string destination;
        private readonly IZipExtractor extractor;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;

        public Fetch(string url, string destination, IZipExtractor extractor,
            IDownloader downloader, IOperationProgress progressObserver, IDeploymentContext deploymentContext,
            IFileSystemOperations fileSystemOperations) : base(deploymentContext, fileSystemOperations)
        {
            this.url = url;
            this.destination = destination;
            this.extractor = extractor;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
        }

        protected override async Task ExecuteCore()
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