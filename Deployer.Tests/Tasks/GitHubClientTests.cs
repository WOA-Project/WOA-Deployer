using System.Threading.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class GitHubClientTests
    {
        [Fact]
        public async Task OpenRelease()
        {
            var client = new GitHubClient();
            await client.OpenRelease("https://github.com/WOA-Project/Lumia950XLPkg", "MSM8994.UEFI.Lumia.950.XL.zip");
        }
    }
}