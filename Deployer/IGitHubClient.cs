using System.IO;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IGitHubClient
    {
        Task<Stream> Open(string repositoryBaseUrl, string branch = "master");
    }
}