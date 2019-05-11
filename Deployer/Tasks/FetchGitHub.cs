using Octokit;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - master")]
    public class FetchGitHub : FetchGitHubBase
    {
        public FetchGitHub(string repositoryUrl, IZipExtractor zipExtractor,
            IDownloader downloader, IGitHubClient gitHubClient, IOperationProgress progressObserver) : base(repositoryUrl,
            "master", zipExtractor, downloader, gitHubClient, progressObserver)
        {
        }
    }
}