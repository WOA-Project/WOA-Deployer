using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class ZipExtractorTests
    {
        [Fact(Skip = "Long running")]
        public async Task RelativeExtract()
        {
            var extractor = new ZipExtractor(new FileSystemOperations());
            var downloader = new GitHubClient();

            using (var stream = await downloader.Open("https://github.com/driver1998/bsp"))
            {
                await extractor.ExtractRelativeFolder(stream, "bsp-master/prebuilt", "Drivers");
            }            
        }
    }
}