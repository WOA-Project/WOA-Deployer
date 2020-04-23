using System.IO;
using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IDownloader
    {
        Task Download(string url, string path, IOperationProgress progressObserver = null, int timeout = 30);

        Task<Stream> GetStream(string url, IOperationProgress progress = null, int timeout = 30);
    }
}