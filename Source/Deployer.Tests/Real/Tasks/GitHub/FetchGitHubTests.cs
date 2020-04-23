using System.Threading.Tasks;
using Deployer.Execution.Testing;
using Deployer.Tasks;
using Deployer.Tests.Real.Tasks.Azure;
using Xunit;

namespace Deployer.Tests.Real.Tasks.GitHub
{
    public class FetchGitHubTests
    {
        [Fact]
        [Trait("Category", "Real")]
        public async Task Test()
        {
            await DownloadMixin.Download(
                "https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers/archive/experimental_keep_out.zip",
                "Reference");

            var task = new FetchGitHub("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers",
                new ZipExtractor(new FileSystemOperations()), null, null, null, new TestDeploymentContext(),
                new TestFileSystemOperations(), new TestOperationContext());
            await task.Execute();

            FileAssertions.AssertEqual("Reference\\MSM8994-8992-NT-ARM64-Drivers-experimental_keep_out",
                "Downloaded\\MSM8994-8992-NT-ARM64-Drivers");
        }
    }   
}