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

            using (var stream = await GitHubMixin.OpenBranchStream("https://github.com/driver1998/bsp"))
            {
                await extractor.ExtractRelativeFolder(stream, "bsp-master/prebuilt", "Drivers");
            }            
        }
    }
}