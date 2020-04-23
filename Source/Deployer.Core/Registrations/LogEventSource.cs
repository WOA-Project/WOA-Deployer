using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Serilog.Events;

namespace Deployer.Core.Registrations
{
    public class LogEventSource
    {
        private readonly Subject<LogEvent> subject = new Subject<LogEvent>();
        private IDisposable subscription;

        public static LogEventSource Current { get; set; } = new LogEventSource();

        public void Connect(IObservable<LogEvent> observable)
        {
            subscription?.Dispose();
            subscription = observable.Subscribe(subject);
        }

        public IObservable<LogEvent> Events => subject.AsObservable();
    }
}