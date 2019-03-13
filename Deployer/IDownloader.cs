using System;
using System.IO;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IDownloader
    {
        Task Download(string url, string path, IDownloadProgress progressObserver = null, int timeout = 30);

        Task<Stream> GetStream(string url, IDownloadProgress progress = null, int timeout = 30);
    }
}