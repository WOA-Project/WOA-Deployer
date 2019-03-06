using System.Reactive.Subjects;
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

            using (var stream =
                await GitHubMixin.GetBranchZippedStream("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers.git",
                    "master", null))
            {
                await extractor.ExtractRelativeFolder(stream, "bsp-master/prebuilt", @"Downloaded\Drivers");
            }
        }
    }
}