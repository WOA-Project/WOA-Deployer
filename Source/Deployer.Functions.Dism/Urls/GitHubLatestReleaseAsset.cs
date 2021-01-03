using System;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Octokit;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Urls
{
    public class GitHubLatestReleaseAsset : DeployerFunction
    {
        private readonly IGitHubClient gitHubClient;

        public GitHubLatestReleaseAsset(IGitHubClient gitHubClient,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.gitHubClient = gitHubClient;
        }

        public async Task<string> Execute(string owner, string repo, string assetName)
        {
            var releases = await gitHubClient.Repository.Release.GetAll(owner, repo);
            var latestRelease = releases.OrderByDescending(x => x.CreatedAt).First();
            var asset = latestRelease.Assets.First(x => string.Equals(x.Name, assetName, StringComparison.OrdinalIgnoreCase));
            return asset.BrowserDownloadUrl;
        }
    }
}