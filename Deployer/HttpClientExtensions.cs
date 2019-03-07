using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;

namespace Deployer
{
    public static class HttpClientExtensions
    {
        private static readonly ByteSize BufferSize = ByteSize.FromKiloBytes(80);

        public static async Task Download(this HttpClient client, string requestUri, Stream destination,
            IObserver<double> progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ObservableMixin.Using(() =>
                    GetStream(client, requestUri, cancellationToken),
                response => Observable.FromAsync(() =>
                    DownloadWithProgressReporting(response, progressObserver, destination, cancellationToken))).RetryWithBackoffStrategy()
                .Catch(DownloadWithIndeterminateSize(client, requestUri, destination, progressObserver, cancellationToken));
        }

        private static IObservable<Unit> DownloadWithIndeterminateSize(HttpClient client, string requestUri, Stream destination,
            IObserver<double> progressObserver,
            CancellationToken cancellationToken)
        {
            return ObservableMixin.Using(() => GetStream(client, requestUri, cancellationToken), message => Observable.FromAsync(async () =>
            {
                progressObserver?.OnNext(double.PositiveInfinity);
                var stream = await message.Content.ReadAsStreamAsync();
                await stream.CopyToAsync(destination);
                progressObserver?.OnNext(double.NaN);
            }));
        }

        private static async Task DownloadWithProgressReporting(HttpResponseMessage response,
            IObserver<double> progressObserver, Stream destination, CancellationToken cancellationToken)
        {
            var contentLength = response.Content.Headers.ContentLength;

            if (contentLength == null)
            {
                throw new InvalidOperationException("Length should not be null");
            }

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                if (progressObserver != null)
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

        private static Task<HttpResponseMessage> GetStream(HttpClient client, string requestUri,
            CancellationToken cancellationToken)
        {
            return client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
    }
}   