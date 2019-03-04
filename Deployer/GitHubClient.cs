using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octokit;

namespace Deployer
{
    public class GitHubClient : IGitHubClient
    {
        public async Task<Stream> Open(string repositoryBaseUrl, string branch = "master")
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var repoInf = GitHubMixin.GetRepoInfo(repositoryBaseUrl);

                var url = $"https://github.com/{repoInf.Owner}/{repoInf.Repository}/archive/{branch}.zip";

                return await client.GetStreamAsync(url);
            }
        }

        public async Task<Stream> OpenRelease(string repositoryBaseUrl, string assetName)
        {
            var octoclient = new Octokit.GitHubClient(new ProductHeaderValue("WOADeployer"));
            var repoInf = GitHubMixin.GetRepoInfo(repositoryBaseUrl);
            var latest = await octoclient.Repository.Release.GetLatest(repoInf.Owner, repoInf.Repository);

            using (var client = new System.Net.Http.HttpClient())
            {
                var asset = latest.Assets.First(x => x.Name == assetName);

                return await client.GetStreamAsync(asset.BrowserDownloadUrl);
            }
        }
    }
}