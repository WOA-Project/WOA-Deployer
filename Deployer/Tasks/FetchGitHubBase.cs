using System;
using System.IO;
using System.Threading.Tasks;
using Octokit;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHubBase : DownloaderTask
    {
        private readonly string repoBaseUrl;
        private readonly string branchName;
        private readonly IZipExtractor extractor;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;
        private readonly IGitHubClient gitHubClient;
        private readonly string repository;
        private readonly RepoInfo repoInfo;
        private const string SubFolder = "Downloaded";

        protected FetchGitHubBase(string repoBaseUrl, string branchName, IZipExtractor extractor, IDownloader downloader,
            IGitHubClient gitHubClient, IOperationProgress progressObserver)
        {
            this.repoBaseUrl = repoBaseUrl;
            this.branchName = branchName;
            this.extractor = extractor;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
            this.gitHubClient = gitHubClient;
            repoInfo = GitHubMixin.GetRepoInfo(repoBaseUrl);
            repository = repoInfo.Repository;
        }

        public override async Task Execute()
        {
            if (Directory.Exists(ArtifactPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", repository);
                return;
            }

            var branch = await gitHubClient.Repository.Branch.Get(repoInfo.Owner, repoInfo.Repository, branchName);
            var commit = branch.Commit;
            var downloadUrl = GitHubMixin.GetCommitDownloadUrl(repoBaseUrl, commit.Sha);

            var downloadeOn = DateTimeOffset.Now;

            using (var stream = await downloader.GetStream(downloadUrl, progressObserver))
            {
                await extractor.ExtractFirstChildToFolder(stream, ArtifactPath, progressObserver);
            }

            SaveMetadata(new
            {
                Commit = commit.Sha,
                Branch = branch.Name,
                DownloadedOn = downloadeOn,
            });
        }

        public override string ArtifactPath => Path.Combine(SubFolder, repoInfo.Repository);
    }
}