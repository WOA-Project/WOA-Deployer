using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHubBase : IDeploymentTask
    {
        private readonly string repoBaseUrl;
        private readonly string branch;
        private readonly IZipExtractor extractor;
        private readonly string repository;
        private readonly string folderPath;
        private const string SubFolder = "Downloaded";

        public FetchGitHubBase(string repoBaseUrl, string branch, IZipExtractor extractor)
        {
            this.repoBaseUrl = repoBaseUrl;
            this.branch = branch;
            this.extractor = extractor;
            var repoInfo = GitHubMixin.GetRepoInfo(repoBaseUrl);
            repository = repoInfo.Repository;
            var folderName = repository;
            folderPath = Path.Combine(SubFolder, folderName);
            
        }

        public async Task Execute()
        {
            if (Directory.Exists(folderPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", repository);
                return;
            }

            var openZipStream = await GitHubMixin.OpenBranchStream(repoBaseUrl, branch);
            using (var stream = openZipStream)
            {
                await extractor.ExtractFirstChildToFolder(stream, folderPath);
            }
        }
    }
}