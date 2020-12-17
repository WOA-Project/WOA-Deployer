using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions.Urls
{
    public class GitHub : DeployerFunction
    {
        public Task<string> Execute(string owner, string repo, string shaOrBranch = "master")
        {
            return Task.FromResult($"https://github.com/{owner}/{repo}/archive/{shaOrBranch}.zip");
        }

        public GitHub(IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
        }
    }
}