using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core;
using Deployer.Core.Scripting;
using Deployer.UI.Properties;
using ReactiveUI;
using Serilog;
using Zafiro.Core.UI;

namespace Deployer.UI
{
    public class ProgressViewModel : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<ByteSize> downloaded;
        private readonly ObservableAsPropertyHelper<bool> isExecuting;
        private readonly ObservableAsPropertyHelper<bool> isProgressIndeterminate;
        private readonly ObservableAsPropertyHelper<bool> isProgressVisible;
        private readonly IOperationContext operationContext;
        private readonly ObservableAsPropertyHelper<double> progress;
        private bool isCloseRequested;
        private readonly IDialogService dialog;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public ProgressViewModel(IReactiveCommand command, IOperationProgress operationProgress, object parent,
            Func<object, IDialogService> dialogFactory, IOperationContext operationContext)
        {
            this.operationContext = operationContext;
            dialog = dialogFactory(parent);
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

            dialog.HandleExceptionsFromCommand(command, HandleException).DisposeWith(disposables);
            command.ThrownExceptions.Subscribe(OnException);
            
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
            disposables.Dispose();
        }

        private (string, string) HandleException(Exception e)
        {
            if (e is FlashException)
            {
                return ( "Error", "Unable to flash the SD Card");
            }

            if (e is TaskCanceledException)
            {
                operationContext.SetOperationAsCancelled();

                return ("Operation cancelled", "The operation has been cancelled");
            }

            return (Resources.ErrorTitle, $"{e.Message}");
        }

        private void OnException(Exception ex)
        {
            if (ex is FlashException)
            {
                Log.Error(ex, "Error while flashing the drive");
            }

            if (ex is TaskCanceledException)
            {
                operationContext.SetOperationAsCancelled();

                if (isCloseRequested)
                {
                    return;
                }

                Log.Error(ex, "Operation cancelled by the user");
            }
            else
            {
                Log.Error(ex, "An error has occurred");
                Log.Information($"Error: {ex.Message}");
                Log.Information("");
            }
        }
    }
}