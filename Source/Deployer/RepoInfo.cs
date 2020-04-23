namespace Deployer
{
    public class RepoInfo
    {
        public string Owner { get; }
        public string Repository { get; }

        public RepoInfo(string owner, string repository)
        {
            Owner = owner;
            Repository = repository;
        }
    }
}