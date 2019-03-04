namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHubBranch : FetchGitHubBase
    {
        public FetchGitHubBranch(string repoBaseUrl, string branch, IGitHubClient client, IZipExtractor extractor) :
            base(repoBaseUrl, branch, client, extractor)
        {
        }
    }
}