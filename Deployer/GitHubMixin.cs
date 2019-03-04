using System.Text.RegularExpressions;

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
    }
}