namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - master")]
    public class FetchGitHub : FetchGitHubBase
    {
        public FetchGitHub(string repoBaseUrl, IZipExtractor extractor, IDownloader downloader, IOperationProgress progressObserver) : base(repoBaseUrl, "master", extractor, downloader, progressObserver)
        {
        }      
    }
}