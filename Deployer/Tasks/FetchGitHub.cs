using Octokit;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - master")]
    public class FetchGitHub : FetchGitHubBase
    {
        public FetchGitHub(string repoBaseUrl, IDeploymentContext context, IZipExtractor extractor,
            IDownloader downloader, IGitHubClient gitHubClient, IOperationProgress progressObserver) : base(repoBaseUrl,
            "master", extractor, downloader, gitHubClient, progressObserver)
        {
        }
    }
}