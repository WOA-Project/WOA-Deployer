using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Scripting;
using Xunit;
using Zafiro.Core.FileSystem;

namespace Deployer.Tests.Real
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
                using (var stream = await downloader.GetStream(GitHubMixin.GetCommitDownloadUrl("https://github.com/driver1998/bsp", "56f3b82d97ab9629689bfe8dad9fbf09fdbd0499")))
                {
                    var relPath = "bsp-56f3b82d97ab9629689bfe8dad9fbf09fdbd0499/prebuilt";
                    await extractor.ExtractRelativeFolder(stream, relPath, "Downloaded\\BSP");
                }
            }
        }
    }
}