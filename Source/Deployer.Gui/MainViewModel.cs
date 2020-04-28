using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Deployer.Core;
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
        private readonly ISubject<string> messages = new Subject<string>();
        private readonly ObservableAsPropertyHelper<bool> isDeploying;
        private CompositeDisposable disposables = new CompositeDisposable();

        public MainViewModel(ICollection<IDetector> detectors, WoaDeployer deployer, IDialogService dialogService, IFilePicker filePicker, OperationProgressViewModel operationProgress)
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

            RunScript = ReactiveCommand.CreateFromObservable(() =>
            {
                var filter = new FileTypeFilter("Deployer Script", new[] {"*.ds", "*.txt"});
                return filePicker
                    .Open("Select a script", new[] {filter})
                    .Where(x => x != null)
                    .SelectMany(file => Observable.FromAsync(() => deployer.RunScript(file.Source.LocalPath)));
            });

            dialogService.HandleExceptionsFromCommand(RunScript);
            dialogService.HandleExceptionsFromCommand(Deploy);
            isDeploying = Deploy.IsExecuting.Merge(RunScript.IsExecuting).ToProperty(this, x => x.IsBusy);
        }

        public ReactiveCommand<Unit, Unit> RunScript { get; }

        public bool IsBusy => isDeploying.Value;

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
}