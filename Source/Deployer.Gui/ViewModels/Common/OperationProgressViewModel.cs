using System;
using System.Linq;
using System.Reactive.Linq;
using ByteSizeLib;
using ReactiveUI;
using Zafiro.Core;

namespace Deployer.Gui.ViewModels.Common
{
    public class OperationProgressViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> isProgressVisible;
        private readonly ObservableAsPropertyHelper<double> progress;
        private readonly ObservableAsPropertyHelper<bool> isProgressIndeterminate;
        private readonly ObservableAsPropertyHelper<ByteSize> downloaded;
        private readonly ObservableAsPropertyHelper<string> message;

        public OperationProgressViewModel(IOperationProgress progress)
        {
            var deployers = Enumerable.Empty<ToDeleteDeployer>();
            message = deployers.ToObservable().SelectMany(x => x.Messages).ToProperty(this, x => x.Message);

            this.progress = progress.Percentage
                .Where(d => !double.IsNaN(d))
                .ToProperty(this, model => model.Progress);

            isProgressVisible = progress.Percentage
                .Select(d => !double.IsNaN(d))
                .ToProperty(this, x => x.IsProgressVisible);

            isProgressIndeterminate = progress.Percentage
                .Select(double.IsPositiveInfinity)
                .ToProperty(this, x => x.IsProgressIndeterminate);

            downloaded = progress.Value
                .Select(x => ByteSize.FromBytes(x))
                .Sample(TimeSpan.FromSeconds(1))
                .ToProperty(this, model =>model.Downloaded);
        }

        public string Message => message.Value;

        public ByteSize Downloaded => downloaded.Value;

        public bool IsProgressIndeterminate => isProgressIndeterminate.Value;

        public bool IsProgressVisible => isProgressVisible.Value;

        public double Progress => progress.Value;
    }
}