using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Interaction;
using Deployer.Gui.ViewModels.Common;
using Deployer.Wpf;
using Grace.DependencyInjection.Attributes;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
using Optional;
using ReactiveUI;
using Serilog;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI;
using Zafiro.UI;

namespace Deployer.Gui.ViewModels.Sections
{
    [Metadata("Name", "Main")]
    [Metadata("Order", 1)]
    public class DeviceDeploymentViewModel : ReactiveObject, ISection
    {
        private readonly IDeviceDeployer deviceDeployer;
        private readonly IInteraction interaction;
        private readonly IDevRepo deviceRepository;
        private Deployment deployment;
        private ObservableAsPropertyHelper<IEnumerable<Deployment>> devices;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public DeviceDeploymentViewModel(IDeviceDeployer deviceDeployer, IInteraction interaction, OperationProgressViewModel operationProgress, IDevRepo deviceRepository)
        {
            OperationProgress = operationProgress;
            this.deviceDeployer = deviceDeployer;
            this.interaction = interaction;
            this.deviceRepository = deviceRepository;

            ConfigureCommands();

            IsBusyObservable = Deploy.IsExecuting;
        }

        public OperationProgressViewModel OperationProgress { get; }

        public Deployment Deployment
        {
            get => deployment;
            set => this.RaiseAndSetIfChanged(ref deployment, value);
        }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> Deploy { get; set; }

        public IEnumerable<Deployment> Deployments => devices.Value;

        public ReactiveCommand<Unit, DeployerStore> FetchDevices { get; set; }

        public IObservable<bool> IsBusyObservable { get; }

        private void ConfigureCommands()
        {
            ConfigureDeployCommand();
            ConfigureFetchDevices();
        }

        private void ConfigureFetchDevices()
        {
            FetchDevices = ReactiveCommand.CreateFromTask(() => deviceRepository.Get());
            devices = FetchDevices.Select(x => x.Deployments).ToProperty(this, model => model.Deployments);
        }
        
        private void ConfigureDeployCommand()
        {
            var hasDevice = this.WhenAnyValue(model => model.Deployment).Select(d => d != null);
            Deploy = ReactiveCommand.CreateFromTask(
                () => deviceDeployer.Deploy(new DeploymentRequest(this.Deployment.Devices.First(), this.deployment.ScriptPath)), hasDevice);

            Deploy.ThrownExceptions.Subscribe(exception => { });

            Deploy
                .Subscribe(either => either
                    .MapRight(success =>
                        interaction.Message("Done", "The deployment has finished successfully", "OK".Some(), Optional.Option.None<string>()))
                    .Handle(deployerError =>
                        interaction.Message("Execution failed", $"The deployment has failed: {deployerError}", "OK".Some(), Optional.Option.None<string>())))
                .DisposeWith(disposables);
        }
    }
}