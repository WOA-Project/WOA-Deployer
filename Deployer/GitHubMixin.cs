using System;
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

        public static async Task<Stream> GetBranchZippedStream(IDownloader downloader, string repositoryBaseUrl, string branch = "master", IDownloadProgress progressObserver = null)
        {
            var repoInf = GetRepoInfo(repositoryBaseUrl);
            var url = $"https://github.com/{repoInf.Owner}/{repoInf.Repository}/archive/{branch}.zip";

            return await downloader.GetStream(url, progressObserver);
        }
    }
}