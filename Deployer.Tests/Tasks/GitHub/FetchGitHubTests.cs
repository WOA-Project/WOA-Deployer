using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks.GitHub
{
    public class FetchGitHubTests
    {       
        [Fact]
        public async Task Test()
        {
            await DownloadMixin.Download(
                "https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers/archive/experimental_keep_out.zip",
                "Reference");

            var task = new FetchGitHub("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers", new ZipExtractor(new FileSystemOperations()), null, null);
            await task.Execute();

            FileAssertions.AssertEqual("Reference\\MSM8994-8992-NT-ARM64-Drivers-experimental_keep_out",
                "Downloaded\\MSM8994-8992-NT-ARM64-Drivers");
        }
    }   
}