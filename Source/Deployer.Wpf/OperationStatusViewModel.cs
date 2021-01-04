using System;
using System.Reactive.Linq;
using ByteSizeLib;
using Deployer.Core;
using ReactiveUI;
using Zafiro.Core;

namespace Deployer.Wpf
{
    public class OperationStatusViewModel : ReactiveObject
    {
        private bool isProgressVisible;
        private bool isProgressIndeterminate;
        private double percentage;

        private readonly ObservableAsPropertyHelper<string> message;

        public OperationStatusViewModel(IWoaDeployer deployer, IOperationProgress operationProgress)
        {
            message = deployer.Messages.ToProperty(this, x => x.Message);

            operationProgress.Progress.Subscribe(progress => {
                switch (progress)
                {
                    case Done done:
                        IsProgressVisible = false;
                        ProgressText = "";
                        break;
                    case Percentage p:
                        IsProgressVisible = true;
                        ProgressText = string.Format("{0:P0}", p.Value);
                        IsProgressIndeterminate = false;
                        Percentage = p.Value;
                        break;
                    case AbsoluteProgress<ulong> undefinedProgress:
                        IsProgressVisible = true;
                        IsProgressIndeterminate = true;

                        ProgressText = string.Format("{0} downloaded", ByteSize.FromBytes(undefinedProgress.Value));
                        break;
                    case Unknown unknown:
                        IsProgressVisible = true;
                        IsProgressIndeterminate = true;
                        ProgressText = "";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(progress));
                }
            });
        }

        private string progressText;

        public string ProgressText
        {
            get => progressText;
            set => this.RaiseAndSetIfChanged(ref progressText, value);
        }

        public string Message => message.Value;

        public bool IsProgressIndeterminate
        {
            get => isProgressIndeterminate;
            set => this.RaiseAndSetIfChanged(ref isProgressIndeterminate, value);
        }

        public bool IsProgressVisible
        {
            get => isProgressVisible;
            set => this.RaiseAndSetIfChanged(ref isProgressVisible, value);
        }

        public double Percentage
        {
            get => percentage;
            set => this.RaiseAndSetIfChanged(ref percentage, value);
        }
    }
}