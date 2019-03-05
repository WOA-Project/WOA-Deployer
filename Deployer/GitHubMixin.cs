using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deployer
{
    public static class GitHubMixin
    {
        public static RepoInfo GetRepoInfo(string repositoryBaseUrl)
        {

            var matches = Regex.Match(repositoryBaseUrl, "https://github\\.com/([\\w-]*)/([\\w-]*)");
            var owner = matches.Groups[1].Value;
            var repository = matches.Groups[2].Value;

            return new RepoInfo(owner, repository);
        }

        public static async Task<Stream> OpenBranchStream(string repositoryBaseUrl, string branch = "master")
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var repoInf = GetRepoInfo(repositoryBaseUrl);

                var url = $"https://github.com/{repoInf.Owner}/{repoInf.Repository}/archive/{branch}.zip";

                var openBranchStream = await client.GetStreamAsync(url);
                return openBranchStream;
            }
        }
    }
}