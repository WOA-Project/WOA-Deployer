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
        private readonly ObservableAsPropertyHelper<bool> isDeploying;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public MainViewModel(ICollection<IDetector> detectors, WoaDeployer deployer, IDialogService dialogService, IFilePicker filePicker, OperationProgressViewModel operationProgress)
        {
            OperationProgress = operationProgress;
            Detect = DeployScript(detectors);

            var hasDevice = this.WhenAnyValue(model => model.Device).Select(d => d != null);
            Detect.Subscribe(detected =>
            {
                Device = detected;
            }).DisposeWith(disposables);

            Detect.SelectMany(async device =>
                {
                    if (device == null)
                    {
                        await dialogService.Notice("Cannot autodetect any device", "Cannot detect any device. Please, select your device manually");
                    }

                    return Unit.Default;
                })
                .Subscribe()
                .DisposeWith(disposables);

            GetRequirements = ReactiveCommand.CreateFromTask(() => deployer.GetRequirements(Device), hasDevice);
            requirements = GetRequirements.ToProperty(this, model => model.Requirements);
            Deploy = DeployCommand(deployer, hasDevice);

            RunScript = RunScriptCommand(deployer, filePicker);

            dialogService.HandleExceptionsFromCommand(RunScript, exception => ("Script execution failed", exception.Message));
            dialogService.HandleExceptionsFromCommand(Deploy, exception =>
            {
                Log.Error(exception, exception.Message);

                if (exception is DeploymentCancelledException)
                {
                    return ("Deployment cancelled", "Deployment cancelled");
                }

                return ("Deployment failed", exception.Message);
            });

            RunScript.SelectMany(async x =>
                {
                    await dialogService.Notice("Execution finished", "Execution finished");
                    return Unit.Default;
                }).Subscribe()
                .DisposeWith(disposables);

            Deploy.SelectMany(async x =>
                {
                    await dialogService.Notice("Deployment finished", "Deployment finished");
                    return Unit.Default;
                }).Subscribe()
                .DisposeWith(disposables);

            isDeploying = Deploy.IsExecuting.Merge(RunScript.IsExecuting).ToProperty(this, x => x.IsBusy);
        }

        private ReactiveCommand<Unit, Unit> DeployCommand(WoaDeployer deployer, IObservable<bool> hasDevice)
        {
            return ReactiveCommand.CreateFromTask(() => deployer.Deploy(Device), hasDevice);
        }

        private static ReactiveCommand<Unit, Device> DeployScript(ICollection<IDetector> detectors)
        {
            return ReactiveCommand.CreateFromTask(async () =>
            {
                var detections = await Task.WhenAll(detectors.Select(d => d.Detect()));
                return detections.FirstOrDefault(d => d != null);
            });
        }

        private static ReactiveCommand<Unit, Unit> RunScriptCommand(WoaDeployer deployer, IFilePicker filePicker)
        {
            return ReactiveCommand.CreateFromObservable(() =>
            {
                var filter = new FileTypeFilter("Deployer Script", new[] { "*.ds", "*.txt" });
                return filePicker
                    .Open("Select a script", new[] { filter })
                    .Where(x => x != null)
                    .SelectMany(file => Observable.FromAsync(() => deployer.RunScript(file.Source.LocalPath)));
            });
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