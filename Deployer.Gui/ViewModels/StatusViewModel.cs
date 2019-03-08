using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using DynamicData;
using ReactiveUI;
using Serilog.Events;

namespace Deployer.Gui.ViewModels
{
    public class StatusViewModel : ReactiveObject, IDisposable
    {
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly ObservableAsPropertyHelper<bool> isProgressIndeterminate;

        private ObservableAsPropertyHelper<RenderedLogEvent> statusHelper;
        private IDisposable logLoader;

        private ReadOnlyObservableCollection<RenderedLogEvent> logEvents;

        public StatusViewModel(IFileSystemOperations fileSystemOperations, IObservable<LogEvent> events, IObservable<double> progressSubject)
        {
            this.fileSystemOperations = fileSystemOperations;
            progressHelper = progressSubject
                .Where(d => !double.IsNaN(d))
                .ToProperty(this, model => model.Progress);

            isProgressVisibleHelper = progressSubject
                .Select(d => !double.IsNaN(d))
                .ToProperty(this, x => x.IsProgressVisible);

            isProgressIndeterminate = progressSubject
                .Select(double.IsPositiveInfinity)
                .ToProperty(this, x => x.IsProgressIndeterminate);

            SetupLogging(events);

            OpenLogFolder = ReactiveCommand.Create(OpenLogs);
        }

        private void OpenLogs()
        {
            fileSystemOperations.EnsureDirectoryExists("Logs");
            Process.Start("Logs");
        }

        public bool IsProgressIndeterminate => isProgressIndeterminate.Value;

        private readonly ObservableAsPropertyHelper<bool> isProgressVisibleHelper;

        public bool IsProgressVisible => isProgressVisibleHelper.Value;

        public ReadOnlyObservableCollection<RenderedLogEvent> Events => logEvents;

        public double Progress => progressHelper.Value;

        public RenderedLogEvent Status => statusHelper.Value;

        private readonly ObservableAsPropertyHelper<double> progressHelper;

        private void SetupLogging(IObservable<LogEvent> events)
        {
            var conn = events
                .ObserveOn(SynchronizationContext.Current)
                .Where(x => x.Level == LogEventLevel.Information)
                .Publish();

            statusHelper = conn
                .Select(RenderedLogEvent)
                .ToProperty(this, x => x.Status);

            logLoader = conn
                .ToObservableChangeSet()
                .Transform(RenderedLogEvent)
                .Bind(out logEvents)
                .DisposeMany()
                .Subscribe();

            conn.Connect();
        }

        public ReactiveCommand<Unit, Unit> OpenLogFolder { get; }
        private static RenderedLogEvent RenderedLogEvent(LogEvent x)
        {
            return new RenderedLogEvent
            {
                Message = x.RenderMessage(),
                Level = x.Level
            };
        }

        public void Dispose()
        {
            isProgressIndeterminate?.Dispose();
            statusHelper?.Dispose();
            logLoader?.Dispose();
            isProgressVisibleHelper?.Dispose();
            progressHelper?.Dispose();
        }
    }
}