using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class FetchGitHubTests
    {
        [Fact(Skip = "Long running")]
        public async Task Test()
        {
            var task = new FetchGitHub("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers",
                new GitHubClient(), new ZipExtractor(new FileSystemOperations()));
            await task.Execute();
        }
    }    
}