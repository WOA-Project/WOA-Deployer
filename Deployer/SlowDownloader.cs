using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Utils;

namespace Deployer
{
    public class SlowDownloader : IDownloader
    {
        private readonly HttpClient client;

        public SlowDownloader(HttpClient client)
        {
            this.client = client;
        }

        public async Task Download(string url, string path, IDownloadProgress progressObserver = null, int timeout = 30)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                await Download(url, fileStream, progressObserver, timeout);
            }
        }

        private async Task Download(string url, Stream destination, IDownloadProgress progressObserver = null,
            int timeout = 30)
        {
            long? totalBytes = 0;
            long bytesWritten = 0;

            await ObservableMixin.Using(() => client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead),
                    s =>
                    {
                        totalBytes = s.Content.Headers.ContentLength;
                        if (!totalBytes.HasValue)
                        {
                            progressObserver?.Percentage.OnNext(double.PositiveInfinity);
                        }
                        return ObservableMixin.Using(() => s.Content.ReadAsStreamAsync(),
                            contentStream => contentStream.ReadToEndObservable());
                    })
                .Do(bytes =>
                {
                    bytesWritten += bytes.Length;
                    if (totalBytes.HasValue)
                    {
                        progressObserver?.Percentage.OnNext((double)bytesWritten / totalBytes.Value);                        
                    }

                    progressObserver?.BytesDownloaded?.OnNext(bytesWritten);
                })
                .Timeout(TimeSpan.FromSeconds(timeout))
                .Select(bytes => Observable.FromAsync(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                    await destination.WriteAsync(bytes, 0, bytes.Length);
                    return Unit.Default;
                }))
                .Merge(1);
        }

        private static readonly int BufferSize = (int)ByteSize.FromKiloBytes(8).Bytes;

        public async Task<Stream> GetStream(string url, IDownloadProgress progress = null, int timeout = 30)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var stream = File.Create(tmpFile, BufferSize, FileOptions.DeleteOnClose);

            await Download(url, stream, progress, timeout);
            return stream;
        }
    }
}