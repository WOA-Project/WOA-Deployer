using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub subfolder: {0}/{1} to {2}")]
    public class GitHubFolderUnpack : IDeploymentTask
    {
        private readonly string url;
        private readonly string relativePath;
        private readonly string destination;
        private readonly IZipExtractor zipExtractor;
        private readonly IGitHubClient gitHubClient;

        public GitHubFolderUnpack(string url, string relativePath, string destination, IZipExtractor zipExtractor, IGitHubClient gitHubClient)
        {
            this.url = url;
            this.relativePath = relativePath;
            this.destination = destination;
            this.zipExtractor = zipExtractor;
            this.gitHubClient = gitHubClient;
        }

        public async Task Execute()
        {
            using (var stream = await gitHubClient.Open(url))
            {
                await zipExtractor.ExtractRelativeFolder(stream, relativePath, destination);
            }
        }
    }
}