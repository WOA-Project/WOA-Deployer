using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using ByteSizeLib;
using Deployer.Tasks;
using DynamicData;
using ReactiveUI;
using Serilog.Events;

namespace Deployer.UI.ViewModels
{
    public class OngoingOperationViewModel : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<ByteSize> downloaded;
        private readonly ObservableAsPropertyHelper<bool> isProgressIndeterminate;

        private ObservableAsPropertyHelper<string> currentActionDetail;
        private IDisposable logLoader;

        private ReadOnlyObservableCollection<RenderedLogEvent> logEvents;

        public OngoingOperationViewModel(IOperationContext context, IObservable<LogEvent> events, IOperationProgress progress)
        {
            progressHelper = progress.Percentage
                .Where(d => !double.IsNaN(d))
                .ToProperty(this, model => model.Progress);

            isProgressVisibleHelper = progress.Percentage
                .Select(d => !double.IsNaN(d))
                .ToProperty(this, x => x.IsProgressVisible);

            isProgressIndeterminate = progress.Percentage
                .Select(double.IsPositiveInfinity)
                .ToProperty(this, x => x.IsProgressIndeterminate);

            downloaded = progress.Value
                .Select(x => ByteSize.FromBytes(x))
                .Sample(TimeSpan.FromSeconds(1))
                .ToProperty(this, model =>model.Downloaded);

            SetupLogging(events);

            CancelCommand = ReactiveCommand.Create(context.Cancel);

            context.Cancelling.Subscribe(unit =>
            {
                IsCancelling = true;
                CancelButtonText = "Cancelling...";
            });
            
            context.Cancelled.Subscribe(_ =>
            {
                IsCancelling = false;
                CancelButtonText = "Cancel";
            });
        }

        public string CancelButtonText
        {
            get => cancelButtonText;
            set => this.RaiseAndSetIfChanged(ref cancelButtonText, value);
        }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        
        public bool IsProgressIndeterminate => isProgressIndeterminate.Value;

        private readonly ObservableAsPropertyHelper<bool> isProgressVisibleHelper;

        public bool IsProgressVisible => isProgressVisibleHelper.Value;

        public ReadOnlyObservableCollection<RenderedLogEvent> Events => logEvents;

        public double Progress => progressHelper.Value;

        public string CurrentActionDetail => currentActionDetail.Value;

        private readonly ObservableAsPropertyHelper<double> progressHelper;
        private ObservableAsPropertyHelper<string> currentActionTitle;
        private string cancelButtonText = "Cancel";
        private bool isCancelling;

        private void SetupLogging(IObservable<LogEvent> events)
        {
            var conn = events
                .ObserveOn(SynchronizationContext.Current)
                .Where(x => x.Level == LogEventLevel.Information)
                .Publish();

            currentActionTitle = conn
                .Select(RenderedLogEvent)
                .Where(x => x.Message.StartsWith("#"))
                .Select(x => x.Message.Substring(1))
                .ToProperty(this, x => x.CurrentActionTitle);

            currentActionDetail = conn
                .Select(RenderedLogEvent)
                .Where(x => !x.Message.StartsWith("#"))
                .Select(x => x.Message)
                .ToProperty(this, x => x.CurrentActionDetail);

            logLoader = conn
                .ToObservableChangeSet()
                .Transform(RenderedLogEvent)
                .Bind(out logEvents)
                .DisposeMany()
                .Subscribe();

            conn.Connect();
        }

        public string CurrentActionTitle => currentActionTitle.Value;

        public ByteSize Downloaded => downloaded.Value;

        public bool IsCancelling
        {
            get => isCancelling;
            set => this.RaiseAndSetIfChanged(ref isCancelling, value);
        }

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
            currentActionDetail?.Dispose();
            logLoader?.Dispose();
            isProgressVisibleHelper?.Dispose();
            progressHelper?.Dispose();
        }
    }
}