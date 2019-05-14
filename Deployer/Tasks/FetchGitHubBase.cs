using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHubBase : DownloaderTask
    {
        private readonly string repositoryUrl;
        private readonly string branchName;
        private readonly IZipExtractor zipExtractor;
        private readonly IDownloader downloader;
        private readonly IOperationProgress progressObserver;
        private readonly IGitHubClient gitHubClient;
        private readonly string repository;
        private readonly RepoInfo repoInfo;

        protected FetchGitHubBase(string repositoryUrl, string branchName, IZipExtractor zipExtractor,
            IDownloader downloader,
            IGitHubClient gitHubClient, IOperationProgress progressObserver, IDeploymentContext deploymentContext,
            IFileSystemOperations fileSystemOperations) : base(deploymentContext, fileSystemOperations)
        {
            this.repositoryUrl = repositoryUrl;
            this.branchName = branchName;
            this.zipExtractor = zipExtractor;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
            this.gitHubClient = gitHubClient;
            repoInfo = GitHubMixin.GetRepoInfo(repositoryUrl);
            repository = repoInfo.Repository;
        }

        protected override string ArtifactName => repoInfo.Repository;

        protected override async Task ExecuteCore()
        {
            if (Directory.Exists(ArtifactPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", repository);
                return;
            }

            var branch = await gitHubClient.Repository.Branch.Get(repoInfo.Owner, repoInfo.Repository, branchName);
            var commit = branch.Commit;
            var downloadUrl = GitHubMixin.GetCommitDownloadUrl(repositoryUrl, commit.Sha);

            var downloadeOn = DateTimeOffset.Now;

            using (var stream = await downloader.GetStream(downloadUrl, progressObserver))
            {
                await zipExtractor.ExtractFirstChildToFolder(stream, ArtifactPath, progressObserver);
            }

            SaveMetadata(new
            {
                Repository = repoInfo,
                Commit = commit.Sha,
                Branch = branch.Name,
                DownloadedOn = downloadeOn,
            });
        }
    }
}