using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class FetchGitHubBranchTests
    {
        [Fact(Skip = "Long running")]
        public async Task Test()
        {
            var task = new FetchGitHubBranch("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers", "master", new ZipExtractor(new FileSystemOperations()), null);
            await task.Execute();
        }
    }
}