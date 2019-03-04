using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub subfolder: {0}/{1} to {2}")]
    public class FetchGitHubFolder : IDeploymentTask
    {
        private readonly string url;
        private readonly string relativePath;
        private readonly string destination;
        private readonly IZipExtractor zipExtractor;
        private readonly IGitHubClient gitHubClient;
        private readonly IFileSystemOperations fileSystemOperations;

        public FetchGitHubFolder(string url, string relativePath, string destination, IZipExtractor zipExtractor,
            IGitHubClient gitHubClient, IFileSystemOperations fileSystemOperations)
        {
            this.url = url;
            this.relativePath = relativePath;
            this.destination = destination;
            this.zipExtractor = zipExtractor;
            this.gitHubClient = gitHubClient;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("{Url}{Folder} was already downloaded. Skipping download.", url, relativePath);
                return;
            }

            using (var stream = await gitHubClient.Open(url))
            {
                await zipExtractor.ExtractRelativeFolder(stream, relativePath, destination);
            }
        }
    }
}