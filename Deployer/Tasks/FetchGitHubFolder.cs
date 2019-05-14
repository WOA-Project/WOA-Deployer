using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub subfolder: {0}/{1} to {2}")]
    public class FetchGitHubFolder : DownloaderTask
    {
        private readonly string repositoryUrl;
        private readonly string relativePath;
        private readonly string destination;
        private readonly IZipExtractor zipExtractor;
        private readonly IDownloader downloader;
        private readonly IGitHubClient gitHubClient;
        private readonly IOperationProgress progressObserver;
        private readonly RepoInfo repoInfo;
        private readonly string branchName;

        public FetchGitHubFolder(string repositoryUrl, string branchName, string relativePath, string destination,
            IZipExtractor zipExtractor, IDownloader downloader, IGitHubClient gitHubClient,
            IOperationProgress progressObserver, IDeploymentContext deploymentContext, IFileSystemOperations fileSystemOperations) : base(deploymentContext, fileSystemOperations)
        {
            this.repositoryUrl = repositoryUrl;
            this.branchName = branchName;
            this.relativePath = relativePath;
            this.destination = destination;
            this.zipExtractor = zipExtractor;
            this.downloader = downloader;
            this.gitHubClient = gitHubClient;
            this.progressObserver = progressObserver;
            repoInfo = GitHubMixin.GetRepoInfo(repositoryUrl);
        }

        protected override string ArtifactName => destination;

        protected override async Task ExecuteCore()
        {
            if (Directory.Exists(ArtifactPath))
            {
                Log.Warning("{Url}{Folder} was already downloaded. Skipping download.", repositoryUrl, relativePath);
                return;
            }

            var branch = await gitHubClient.Repository.Branch.Get(repoInfo.Owner, repoInfo.Repository, branchName);
            var commit = branch.Commit;
            var downloadUrl = GitHubMixin.GetCommitDownloadUrl(repositoryUrl, commit.Sha);

            var downloadeOn = DateTimeOffset.Now;

            using (var stream = await downloader.GetStream(downloadUrl, progressObserver))
            {
                await zipExtractor.ExtractRelativeFolder(stream, relativePath, ArtifactPath);
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