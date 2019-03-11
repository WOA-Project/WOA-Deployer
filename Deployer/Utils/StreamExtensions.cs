using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Deployer.Utils
{
    public static class StreamExtensions
    {
        internal const int defaultBufferSize = 4096;

        public static IObservable<byte[]> ReadToEndObservable(this Stream stream)
        {
            return stream.ReadToEndObservable(new byte[defaultBufferSize]);
        }

        /// <summary>
        /// Creates an observable sequence by asynchronously reading bytes from the current position to the end of the specified <paramref name="stream"/>
        /// and advances the position within the stream to the end.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to be read.</param>
        /// <param name="bufferSize">The maximum length of each byte array that is read.</param>
        /// <returns>An observable sequence of byte arrays of the specified maximum size read from the current position to the end of the 
        /// specified <paramref name="stream"/>.</returns>
        public static IObservable<byte[]> ReadToEndObservable(this Stream stream, int bufferSize)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(bufferSize > 0);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return stream.ReadToEndObservable(new byte[bufferSize]);
        }

        internal static IObservable<byte[]> ReadToEndObservable(this Stream stream, byte[] buffer)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(buffer != null);
            Contract.Requires(buffer.Length > 0);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return Observable.Create<byte[]>(
                observer =>
                {
                    var subscription = new SerialDisposable();

                    return new CompositeDisposable(
                        subscription,
                        Scheduler.Immediate.Schedule(
                            self =>
                            {
                                bool continueReading = true;

                                subscription.SetDisposableIndirectly(() =>
                                    stream.ReadObservable(buffer).SubscribeSafe(
                                        data =>
                                        {
                                            if (data.Length > 0)
                                            {
                                                observer.OnNext(data);
                                            }
                                            else
                                            {
                                                continueReading = false;
                                            }
                                        },
                                        observer.OnError,
                                        () =>
                                        {
                                            if (continueReading)
                                            {
                                                self();
                                            }
                                            else
                                            {
                                                observer.OnCompleted();
                                            }
                                        }));
                            }));
                });
        }

        internal static IObservable<byte[]> ReadObservable(this Stream stream, byte[] buffer)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(buffer != null);
            Contract.Requires(buffer.Length > 0);
            Contract.Ensures(Contract.Result<IObservable<byte[]>>() != null);

            return stream.ReadObservable(buffer, 0, buffer.Length).Select(
                read =>
                {
                    byte[] data;

                    if (read <= 0)
                    {
                        data = new byte[0];
                    }
                    else if (read == buffer.Length)
                    {
                        data = (byte[]) buffer.Clone();
                    }
                    else
                    {
                        data = new byte[read];

                        Array.Copy(buffer, data, read);
                    }

                    return data;
                });
        }

        public static IObservable<int> ReadObservable(this Stream stream, byte[] buffer, int offset, int count)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(buffer != null);
            Contract.Requires(offset >= 0);
            Contract.Requires(count >= 0);
            Contract.Requires(offset + count <= buffer.Length);
            Contract.Ensures(Contract.Result<IObservable<int>>() != null);

#if !SILVERLIGHT || UNIVERSAL
            return Observable.StartAsync(cancel => stream.ReadAsync(buffer, offset, count, cancel));
#else
      return Task.Factory.FromAsync<byte[], int, int, int>(
        stream.BeginRead,
        stream.EndRead,
        buffer,
        offset,
        count,
        state: null)
        .ToObservable();
#endif
        }
    }
}