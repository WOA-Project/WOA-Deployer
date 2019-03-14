using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests
{
    public class ZipExtractorTests
    {
        [Fact]
        [Trait("Category", "Real")]
        public async Task RelativeExtract()
        {
            var extractor = new ZipExtractor(new FileSystemOperations());

            using (var httpClient = new HttpClient())
            {                
                var downloader = new Downloader(httpClient);
                var stream = await GitHubMixin.GetBranchZippedStream(downloader, "https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers.git");
                using (stream)
                {
                    await extractor.ExtractRelativeFolder(stream, "bsp-master/prebuilt", @"Downloaded\Drivers");
                }
            }
        }
    }
}