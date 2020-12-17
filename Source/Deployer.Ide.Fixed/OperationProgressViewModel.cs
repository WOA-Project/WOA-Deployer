using System;
using System.Reactive.Linq;
using ByteSizeLib;
using Deployer.Net4x;
using ReactiveUI;
using Zafiro.Core;

namespace Deployer.Ide
{
    public class OperationProgressViewModel : ReactiveObject
    {
        private bool isProgressVisible;
        private double percentage;
        private bool isProgressIndeterminate;
        private ByteSize downloaded;
        private readonly ObservableAsPropertyHelper<string> message;

        public OperationProgressViewModel(WoaDeployerBase deployer, IOperationProgress progress)
        {
            message = deployer.Messages.ToProperty(this, x => x.Message);

            progress.Progress.Subscribe(progress => {
                switch (progress)
                {
                    case Done done:
                        IsProgressVisible = false;
                        break;
                    case Percentage p:
                        IsProgressVisible = true;
                        this.Progress = p.Value;
                        break;
                    case UndefinedProgress<ulong> undefinedProgress:
                        IsProgressVisible = true;
                        this.Downloaded = ByteSize.FromBytes(undefinedProgress.Value);
                        break;
                    case Unknown unknown:
                        IsProgressVisible = true;
                        IsProgressIndeterminate = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(progress));
                }
            });
        }

        public string Message => message.Value;

        public ByteSize Downloaded
        {
            get => downloaded;
            set => this.RaiseAndSetIfChanged(ref downloaded, value);
        }

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

        public double Progress
        {
            get => percentage;
            set => this.RaiseAndSetIfChanged(ref percentage, value);
        }
    }
}