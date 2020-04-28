using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Deployer.Core.Utils
{
    public static class NewStreamMixin
    {
        private const int DefaultBufferSize = 4096;

        public static IObservable<byte[]> ReadToEndObservable(this Stream stream, int bufferSize = DefaultBufferSize)
            =>
                Observable.Defer<byte[]>(() =>
                {
                    var bytesRead = -1;
                    var bytes = new byte[bufferSize];
                    return
                        Observable.While(
                            () => bytesRead != 0,
                            Observable
                                .FromAsync(() => stream.ReadAsync(bytes, 0, bufferSize))
                                .Do(x =>
                                {
                                    bytesRead = x;
                                })
                                .Select(x => bytes.Take(x).ToArray()));
                });
    }
}