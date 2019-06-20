using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Tasks;
using Deployer.UI.Properties;
using ReactiveUI;
using Serilog;

namespace Deployer.UI
{
    public class ProgressViewModel : ReactiveObject, IDisposable
    {
        private readonly IContextDialog dialog;
        private readonly ObservableAsPropertyHelper<ByteSize> downloaded;
        private readonly ObservableAsPropertyHelper<bool> isExecuting;
        private readonly ObservableAsPropertyHelper<bool> isProgressIndeterminate;
        private readonly ObservableAsPropertyHelper<bool> isProgressVisible;
        private readonly IOperationContext operationContext;
        private readonly object parent;

        private readonly ObservableAsPropertyHelper<double> progress;
        private readonly IDisposable throwExceptionsHandler;
        private bool isCloseRequested;

        public ProgressViewModel(IReactiveCommand command, IOperationProgress operationProgress, object parent,
            IContextDialog dialog, IOperationContext operationContext)
        {
            this.parent = parent;
            this.dialog = dialog;
            this.operationContext = operationContext;
            Command = command;
            progress = operationProgress.Percentage
                .Where(d => !double.IsNaN(d))
                .ToProperty(this, model => model.Progress);

            isProgressVisible = operationProgress.Percentage
                .Select(d => !double.IsNaN(d))
                .CombineLatest(command.IsExecuting, (isNumber, isExecuting) => isNumber && isExecuting)
                .ToProperty(this, x => x.IsProgressVisible);

            isProgressIndeterminate = operationProgress.Percentage
                .Select(double.IsPositiveInfinity)
                .ToProperty(this, x => x.IsProgressIndeterminate);

            downloaded = operationProgress.Value
                .Select(x => ByteSize.FromBytes(x))
                .Sample(TimeSpan.FromSeconds(1))
                .ToProperty(this, model => model.Downloaded);

            isExecuting = command.IsExecuting
                .ToProperty(this, x => x.IsExecuting);
            throwExceptionsHandler = command.ThrownExceptions.Subscribe(async e => await HandleException(e));
            MessageBus.Current.Listen<CloseMessage>().Subscribe(x => isCloseRequested = true);
        }

        public IReactiveCommand Command { get; }

        public bool IsExecuting => isExecuting.Value;

        public ByteSize Downloaded => downloaded.Value;

        public bool IsProgressIndeterminate => isProgressIndeterminate.Value;

        public bool IsProgressVisible => isProgressVisible.Value;

        public double Progress => progress.Value;

        public void Dispose()
        {
            progress?.Dispose();
            isProgressVisible?.Dispose();
            isProgressIndeterminate?.Dispose();
            downloaded?.Dispose();
            isExecuting?.Dispose();
            Command?.Dispose();
            throwExceptionsHandler.Dispose();
        }

        private async Task HandleException(Exception e)
        {
            if (e is FlashException)
            {
                Log.Error(e, "Error while flashing the drive");
                await dialog.ShowAlert(parent, "Error", "Unable to flash the SD Card");
            }

            if (e is TaskCanceledException)
            {
                operationContext.SetOperationAsCancelled();

                if (isCloseRequested)
                {
                    return;
                }

                Log.Error(e, "Operation cancelled by the user");
                await dialog.ShowAlert(parent, "Operation cancelled", "The operation has been cancelled");
            }
            else
            {
                Log.Error(e, "An error has occurred");
                Log.Information($"Error: {e.Message}");
                await dialog.ShowAlert(parent, Resources.ErrorTitle, $"{e.Message}");
                Log.Information("");
            }
        }
    }
}