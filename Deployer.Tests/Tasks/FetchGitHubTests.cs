using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class FetchGitHubTests
    {       
        [Fact]
        public async Task Test()
        {
            await DownloadMixin.Download(
                "https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers/archive/experimental_keep_out.zip",
                "Reference");

            var task = new FetchGitHubBranch("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers",
                "experimental_keep_out", new ZipExtractor(new FileSystemOperations()), null);
            await task.Execute();

            FileAssertions.AssertEqual("Reference\\MSM8994-8992-NT-ARM64-Drivers-experimental_keep_out",
                "Downloaded\\MSM8994-8992-NT-ARM64-Drivers");
        }
    }   
}