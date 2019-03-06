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
        private readonly IObserver<double> progressObserver;

        public FetchGitHubFolder(string url, string relativePath, string destination, IZipExtractor zipExtractor,
            IFileSystemOperations fileSystemOperations, IObserver<double> progressObserver)
        {
            this.url = url;
            this.relativePath = relativePath;
            this.destination = destination;
            this.zipExtractor = zipExtractor;
            this.fileSystemOperations = fileSystemOperations;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("{Url}{Folder} was already downloaded. Skipping download.", url, relativePath);
                return;
            }

            using (var stream = await GitHubMixin.GetBranchZippedStream(url, progressObserver: progressObserver))
            {
                await zipExtractor.ExtractRelativeFolder(stream, relativePath, destination);
            }
        }
    }
}