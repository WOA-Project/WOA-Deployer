using System;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - master")]
    public class FetchGitHub : FetchGitHubBase
    {
        public FetchGitHub(string repoBaseUrl, IZipExtractor extractor, IObserver<double> progressObserver) : base(repoBaseUrl, "master", extractor, progressObserver)
        {
        }      
    }
}