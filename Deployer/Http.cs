using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deployer
{
    public static class Http
    {
        public static async Task<Stream> GetStream(string url, IObserver<double> progress = null)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var stream = File.Create(tmpFile, 8096, FileOptions.DeleteOnClose);

            using (var client = new HttpClient())
            {
                await client.DownloadAsync(url, stream, progress);
                return stream;
            }
        }
    }
}