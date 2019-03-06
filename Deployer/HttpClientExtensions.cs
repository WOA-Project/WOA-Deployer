using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deployer
{
    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IObserver<double> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var contentLength = response.Content.Headers.ContentLength;

                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }

                    await download.CopyToAsync(destination, 81920, contentLength.Value, progress, cancellationToken);
                    progress.OnNext(double.NaN);
                }
            }
        }

        public static async Task<Stream> Download(string url, IObserver<double> progress)
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