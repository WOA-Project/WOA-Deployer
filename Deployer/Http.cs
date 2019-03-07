using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer
{
    public static class Http
    {
        private static readonly int BufferSize = (int)ByteSize.FromKiloBytes(8).Bytes;

        public static async Task<Stream> GetStream(string url, IObserver<double> progress = null)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var stream = File.Create(tmpFile, BufferSize, FileOptions.DeleteOnClose);

            using (var client = new HttpClient { Timeout = TimeSpan.FromHours(1) })
            {
                await client.Download(url, stream, progress);
                return stream;
            }
        }
    }
}