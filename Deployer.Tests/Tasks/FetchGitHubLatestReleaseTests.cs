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
            var task = new FetchGitHubLatestReleaseAsset("https://github.com/WOA-Project/Lumia950XLPkg", "MSM8994.UEFI.Lumia.950.XL.zip", zipExtractor, null);
            await task.Execute();
        }
    }
}