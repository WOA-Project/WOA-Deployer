using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Exceptions;
using Deployer.Gui.Properties;
using ReactiveUI;
using Serilog;
using Zafiro.Core.UI;

namespace Deployer.Gui
{
    public class MainViewModel : ReactiveObject
    {
        public OperationProgressViewModel OperationProgress { get; }
        private Device device;
        private readonly ObservableAsPropertyHelper<IEnumerable<string>> requirements;
        private readonly ObservableAsPropertyHelper<string> message;
        private readonly ISubject<string> messages = new Subject<string>();
        private readonly ObservableAsPropertyHelper<bool> isDeploying;
        private CompositeDisposable disposables = new CompositeDisposable();

        public MainViewModel(ICollection<IDetector> detectors, WoaDeployer deployer, IDialogService dialogService, OperationProgressViewModel operationProgress)
        {
            OperationProgress = operationProgress;
            Detect = ReactiveCommand.CreateFromTask(async () =>
            {
                var detections = await Task.WhenAll(detectors.Select(d => d.Detect()));
                return detections.FirstOrDefault(d => d != null);
            });

            var hasDevice = this.WhenAnyValue(model => model.Device).Select(d => d != null);
            Detect.Subscribe(detected =>
            {
                Device = detected;
            }).DisposeWith(disposables);

            Detect.SelectMany(async d =>
                {
                    if (d == null)
                        await dialogService.Notice("Cannot autodetect any device",
                            "Cannot detect any device. Please, select your device manually");

                    return Unit.Default;
                })
                .Subscribe()
                .DisposeWith(disposables);

            GetRequirements = ReactiveCommand.CreateFromTask(() => deployer.GetRequirements(Device), hasDevice);
            requirements = GetRequirements.ToProperty(this, model => model.Requirements);
            Deploy = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    await deployer.Deploy(Device);
                    messages.OnNext("Deployment finished!");
                }
                catch (DeploymentCancelledException)
                {
                    messages.OnNext("Deployment cancelled");
                    Log.Information("Deployment cancelled");
                }
                catch (Exception e)
                {
                    messages.OnNext("Deployment failed");
                    Log.Error(e, "Deployment failed");
                }
            }, hasDevice);

            dialogService.HandleExceptionsFromCommand(Deploy);
            message = deployer.Messages.Merge(messages).ToProperty(this, x => x.Message);
            isDeploying = Deploy.IsExecuting.ToProperty(this, x => x.IsDeploying);
        }

        public bool IsDeploying => isDeploying.Value;

        public string Message => message.Value;

        public IEnumerable<string> Requirements => requirements.Value;

        public ReactiveCommand<Unit, IEnumerable<string>> GetRequirements { get; }

        public Device Device
        {
            get => device;
            set => this.RaiseAndSetIfChanged(ref device, value);
        }

        public ReactiveCommand<Unit, Unit> Deploy { get; }

        public ICollection<Device> Devices => Device.KnownDevices;

        public ReactiveCommand<Unit, Device> Detect { get; set; }

        public string Title => AppProperties.AppTitle;
    }

    public class AppProperties
    {
        public const string GitHubBaseUrl = "https://github.com/WOA-Project/WOA-Deployer";
        public static string AppTitle => string.Format(Resources.AppTitle, AppVersionMixin.VersionString);
    }
}