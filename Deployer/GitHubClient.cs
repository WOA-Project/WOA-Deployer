using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deployer
{
    public class GitHubClient : IGitHubClient
    {
        public async Task<Stream> Open(string repositoryBaseUrl, string branch = "master")
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var matches = Regex.Match(repositoryBaseUrl, "https://github\\.com/([\\w-]*)/([\\w-]*)");
                var username = matches.Groups[1].Value;
                var repository = matches.Groups[2].Value;

                var url = $"https://github.com/{username}/{repository}/archive/{branch}.zip";

                var openZipStream = await client.GetStreamAsync(url);
                return openZipStream;
            }
        }
    }
}