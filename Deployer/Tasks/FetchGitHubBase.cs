using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHubBase : IDeploymentTask
    {
        private readonly string repoBaseUrl;
        private readonly string branch;
        private readonly IZipExtractor extractor;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;
        private readonly string repository;
        private readonly string folderPath;
        private const string SubFolder = "Downloaded";

        public FetchGitHubBase(string repoBaseUrl, string branch, IZipExtractor extractor, IDownloader downloader,
            IOperationProgress progressObserver)
        {
            this.repoBaseUrl = repoBaseUrl;
            this.branch = branch;
            this.extractor = extractor;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
            var repoInfo = GitHubMixin.GetRepoInfo(repoBaseUrl);
            repository = repoInfo.Repository;
            var folderName = repository;
            folderPath = Path.Combine(SubFolder, folderName);            
        }

        public async Task Execute()
        {
            if (Directory.Exists(folderPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", repository);
                return;
            }

            var openZipStream = await GitHubMixin.GetBranchZippedStream(downloader, repoBaseUrl, branch, progressObserver);
            using (var stream = openZipStream)
            {
                await extractor.ExtractFirstChildToFolder(stream, folderPath, progressObserver);
            }
        }
    }
}