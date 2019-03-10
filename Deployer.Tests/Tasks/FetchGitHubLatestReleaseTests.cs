using System.Threading.Tasks;
using Deployer.Tasks;
using Octokit;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class FetchGitHubLatestReleaseTests
    {
        [Fact]
        public async Task Test()
        {
            await DownloadMixin.Download("https://github.com/WOA-Project/Lumia950XLPkg/releases/download/1.20/MSM8994.UEFI.Lumia.950.XL.zip", "Reference\\MSM8994.UEFI.Lumia.950.XL");

            var zipExtractor = new ZipExtractor(new FileSystemOperations());
            var task = new FetchGitHubLatestReleaseAsset("https://github.com/WOA-Project/Lumia950XLPkg",
                "MSM8994.UEFI.Lumia.950.XL.zip", zipExtractor, new GitHubClient(new ProductHeaderValue("WOADeployer")),
                null);

            await task.Execute();

            FileAssertions.AssertEqual("Downloaded\\MSM8994.UEFI.Lumia.950.XL", "Reference\\MSM8994.UEFI.Lumia.950.XL\\MSM8994 UEFI (Lumia 950 XL)");
        }
    }
}