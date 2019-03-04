using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class FetchGitHubLatestReleaseTests
    {
        [Fact(Skip = "Long running")]
        public async Task Test()
        {
            var zipExtractor = new ZipExtractor(new FileSystemOperations());
            var task = new FetchGitHubLatestRelease("https://github.com/WOA-Project/Lumia950XLPkg", "MSM8994.UEFI.Lumia.950.XL.zip", zipExtractor);
            await task.Execute();
        }
    }

    public class FetchGitHubBranchTests
    {
        [Fact]
        public async Task Test()
        {
            var task = new FetchGitHubBranch("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers", "master", new GitHubClient(), new ZipExtractor(new FileSystemOperations()));
            await task.Execute();
        }
    }
}