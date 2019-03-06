using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer
{
    public static class HttpClientExtensions
    {
        private static readonly ByteSize BufferSize = ByteSize.FromKiloBytes(80);

        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IObserver<double> progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var contentLength = response.Content.Headers.ContentLength;

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    if (progressObserver != null && contentLength.HasValue)
                    {
                        await stream.CopyTo(destination, (int) BufferSize.Bytes, contentLength.Value, progressObserver,
                            cancellationToken);
                        progressObserver.OnNext(double.NaN);
                    }
                    else
                    {
                        await stream.CopyToAsync(destination);
                    }
                }
            }
        }
    }
}