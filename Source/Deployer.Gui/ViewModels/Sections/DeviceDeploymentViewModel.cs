using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Interaction;
using Deployer.Wpf;
using Grace.DependencyInjection.Attributes;
using Iridio.Runtime.ReturnValues;
using Optional;
using ReactiveUI;
using Zafiro.Core.Patterns.Either;
using Zafiro.UI;
using Option = Optional.Option;

namespace Deployer.Gui.ViewModels.Sections
{
    [Metadata("Name", "Main")]
    [Metadata("Order", 1)]
    public class DeviceDeploymentViewModel : ReactiveObject, ISection
    {
        private readonly IDeviceDeployer deviceDeployer;
        private readonly IDeploymentLibrary deploymentLibrary;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IInteraction interaction;
        private DeploymentDto deployment;
        private ObservableAsPropertyHelper<IEnumerable<DeploymentDto>> deployments;
        private ObservableAsPropertyHelper<IEnumerable<DeviceDto>> devices;

        public DeviceDeploymentViewModel(IDeviceDeployer deviceDeployer, IInteraction interaction,
            OperationProgressViewModel operationProgress, IDeploymentLibrary deploymentLibrary)
        {
            OperationProgress = operationProgress;
            this.deviceDeployer = deviceDeployer;
            this.interaction = interaction;
            this.deploymentLibrary = deploymentLibrary;

            ConfigureCommands();

            IsBusyObservable = Deploy.IsExecuting;
        }

        public OperationProgressViewModel OperationProgress { get; }

        public DeploymentDto Deployment
        {
            get => deployment;
            set => this.RaiseAndSetIfChanged(ref deployment, value);
        }

        private DeviceDto device;

        public DeviceDto Device
        {
            get => device;
            set => this.RaiseAndSetIfChanged(ref device, value);
        }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> Deploy { get; set; }

        public IEnumerable<DeploymentDto> Deployments => deployments.Value;

        public IEnumerable<DeviceDto> Devices => devices.Value;

        public IObservable<bool> IsBusyObservable { get; }

        private void ConfigureCommands()
        {
            ConfigureDeployCommand();

            FetchDevices = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Devices());
            devices = FetchDevices.ToProperty(this, x => x.Devices);
            FetchDeployments = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Deployments());
            deployments = FetchDeployments.ToProperty(this, x => x.Deployments);
        }

        public ReactiveCommand<Unit, List<DeploymentDto>> FetchDeployments { get; set; }

        public ReactiveCommand<Unit, List<DeviceDto>> FetchDevices { get; set; }

        public ReactiveCommand<Unit, Unit> Fetch { get; set; }

        private void ConfigureDeployCommand()
        {
            var hasDevice = this.WhenAnyValue(model => model.Deployment).Select(d => d != null);
            Deploy = ReactiveCommand.CreateFromTask(
                () => deviceDeployer.Deploy(new DeploymentRequest(Device, deployment.ScriptPath)),
                hasDevice);

            Deploy.ThrownExceptions.Subscribe(exception => { });

            Deploy
                .Subscribe(either => either
                    .MapRight(success =>
                        interaction.Message("Done", "The deployment has finished successfully", "OK".Some(),
                            Option.None<string>()))
                    .Handle(deployerError =>
                        interaction.Message("Execution failed", $"The deployment has failed: {deployerError}",
                            "OK".Some(), Option.None<string>())))
                .DisposeWith(disposables);
        }
    }
}