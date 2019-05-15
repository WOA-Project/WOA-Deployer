using System.Threading;
using Octokit;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - {1}")]
    public class FetchGitHubBranch : FetchGitHubBase
    {
        public FetchGitHubBranch(string repositoryUrl, string branchName,
            IZipExtractor zipExtractor, IDownloader downloader, IGitHubClient gitHubClient,
            IOperationProgress progressObserver, IDeploymentContext deploymentContext,
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) :
            base(repositoryUrl, branchName, zipExtractor, downloader, gitHubClient, progressObserver, deploymentContext, fileSystemOperations, operationContext)
        {
        }
    }
}