using System.Threading;
using System.Threading.Tasks;
using Deployer.Execution.Testing;
using Deployer.Tasks;
using Deployer.Tests.Real.Tasks.Azure;
using Octokit;
using Xunit;

namespace Deployer.Tests.Real.Tasks.GitHub
{
    public class FetchGitHubLatestReleaseTests
    {
        [Fact]
        [Trait("Category", "Real")]
        public async Task Test()
        {
            await DownloadMixin.Download("https://github.com/WOA-Project/Lumia950XLPkg/releases/download/1.20/MSM8994.UEFI.Lumia.950.XL.zip", "Reference\\MSM8994.UEFI.Lumia.950.XL");

            var zipExtractor = new ZipExtractor(new FileSystemOperations());
            var task = new FetchGitHubLatestReleaseAsset("https://github.com/WOA-Project/Lumia950XLPkg",
                "MSM8994.UEFI.Lumia.950.XL.zip", zipExtractor, new GitHubClient(new ProductHeaderValue("WOADeployer")),
                null, null, new TestDeploymentContext(), new TestFileSystemOperations(), new TestOperationContext());

            await task.Execute();

            FileAssertions.AssertEqual("Downloaded\\MSM8994.UEFI.Lumia.950.XL", "Reference\\MSM8994.UEFI.Lumia.950.XL\\MSM8994 UEFI (Lumia 950 XL)");
        }
    }
}