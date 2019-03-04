using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class FetchGitHub : FetchGitHubBase
    {
        public FetchGitHub(string repoBaseUrl, IGitHubClient client, IZipExtractor extractor) : base(repoBaseUrl, "master", client, extractor)
        {
        }      
    }
}