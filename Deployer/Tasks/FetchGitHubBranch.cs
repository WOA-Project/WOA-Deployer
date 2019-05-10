using Octokit;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - {1}")]
    public class FetchGitHubBranch : FetchGitHubBase
    {
        public FetchGitHubBranch(string repoBaseUrl, string branchName, IDeploymentContext context,
            IZipExtractor extractor, IDownloader downloader, IGitHubClient gitHubClient,
            IOperationProgress progressObserver) :
            base(repoBaseUrl, branchName, extractor, downloader, gitHubClient, progressObserver)
        {
        }
    }
}