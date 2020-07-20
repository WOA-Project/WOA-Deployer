using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Deployer.Core;
using ReactiveUI;
using Serilog;
using Zafiro.Core.UI;

namespace Deployer.Gui
{
    public class MainViewModel : ReactiveObject
    {
        private readonly WoaDeployer deployer;
        private readonly IDialogService dialogService;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IFilePicker filePicker;
        private readonly IDeviceRepository deviceRepository;
        private readonly ObservableAsPropertyHelper<bool> isDeploying;
        private Device device;
        private ObservableAsPropertyHelper<IEnumerable<Device>> devices;

        public MainViewModel(WoaDeployer deployer, IDialogService dialogService,
            IFilePicker filePicker, IDeviceRepository deviceRepository, OperationProgressViewModel operationProgress)
        {
            this.deployer = deployer;
            this.dialogService = dialogService;
            this.filePicker = filePicker;
            this.deviceRepository = deviceRepository;
            OperationProgress = operationProgress;

            ConfigureCommands();

            isDeploying = Deploy.IsExecuting.Merge(RunScript.IsExecuting).ToProperty(this, x => x.IsBusy);
        }

        public OperationProgressViewModel OperationProgress { get; }

        public ReactiveCommand<Unit, Unit> RunScript { get; set; }

        public bool IsBusy => isDeploying.Value;

        public Device Device
        {
            get => device;
            set => this.RaiseAndSetIfChanged(ref device, value);
        }

        public ReactiveCommand<Unit, Unit> Deploy { get; set; }

        public IEnumerable<Device> Devices => devices.Value;

        public ReactiveCommand<Unit, Device> Detect { get; set; }

        public string Title => AppProperties.AppTitle;

        private void ConfigureCommands()
        {
            ConfigureDeployCommand();
            ConfigureRunCommand();
            ConfigureFetchDevices();
        }

        private void ConfigureFetchDevices()
        {
            FetchDevices = ReactiveCommand.CreateFromTask(() => deviceRepository.GetAll());
            devices = FetchDevices.ToProperty(this, model => model.Devices);
            dialogService.HandleExceptionsFromCommand(FetchDevices, "Error", "Cannot fetch supported devices");
        }

        public ReactiveCommand<Unit, IEnumerable<Device>> FetchDevices { get; set; }

        private void ConfigureDeployCommand()
        {
            var hasDevice = this.WhenAnyValue(model => model.Device).Select(d => d != null);
            Deploy = ReactiveCommand.CreateFromTask(() => deployer.Deploy(Device), hasDevice);
            dialogService.HandleExceptionsFromCommand(Deploy, exception =>
            {
                Log.Error(exception, exception.Message);

                if (exception is DeploymentCancelledException)
                {
                    return ("Deployment cancelled", "Deployment cancelled");
                }

                return ("Deployment failed", exception.Message);
            });

            Deploy.OnSuccess(() => dialogService.Notice("Deployment finished", "Deployment finished")).DisposeWith(disposables);
        }

        private void ConfigureRunCommand()
        {
            RunScript = RunScriptCommand(deployer, filePicker);

            RunScript
                .OnSuccess(() => dialogService.Notice("Execution finished", "The script has been executed successfully"))
                .DisposeWith(disposables);

            dialogService.HandleExceptionsFromCommand(RunScript,
                exception => ("Script execution failed", exception.Message));
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
                var filter = new FileTypeFilter("Deployer Script", "*.ds", "*.txt");
                return filePicker
                    .Open("Select a script", new[] {filter})
                    .Where(x => x != null)
                    .SelectMany(file => Observable.FromAsync(() => deployer.RunScript(file.Source.LocalPath)));
            });
        }
    }
}