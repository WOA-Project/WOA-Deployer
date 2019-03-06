using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Execution;
using Octokit;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching asset {1} from latest release at {0}")]
    public class FetchGitHubLatestReleaseAsset : IDeploymentTask
    {
        private readonly string repoUrl;
        private readonly string assetName;
        private readonly IZipExtractor extractor;
        private readonly IObserver<double> progressObserver;
        private readonly string folderPath;
        private const string SubFolder = "Downloaded";

        public FetchGitHubLatestReleaseAsset(string repoUrl, string assetName, IZipExtractor extractor, IObserver<double> progressObserver)
        {
            this.repoUrl = repoUrl ?? throw new ArgumentNullException(nameof(repoUrl));
            this.assetName = assetName ?? throw new ArgumentNullException(nameof(assetName));
            this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            this.progressObserver = progressObserver;

            folderPath = Path.Combine(SubFolder, Path.GetFileNameWithoutExtension(assetName));
        }

        public async Task Execute()
        {
            if (Directory.Exists(folderPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", folderPath);
                return;
            }

            var gitHubClient = new Octokit.GitHubClient(new ProductHeaderValue("WOADeployer"));
            var repoInf = GitHubMixin.GetRepoInfo(repoUrl);
            var latest = await gitHubClient.Repository.Release.GetLatest(repoInf.Owner, repoInf.Repository);
            var asset = latest.Assets.First(x => string.Equals(x.Name, assetName, StringComparison.OrdinalIgnoreCase));
            using (var stream = await HttpClientExtensions.Download(asset.BrowserDownloadUrl, progressObserver))
            {
                await extractor.ExtractFirstChildToFolder(stream, folderPath);
            }
        }
    }
}