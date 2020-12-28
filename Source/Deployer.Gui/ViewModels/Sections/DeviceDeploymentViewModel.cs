using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Interaction;
using Deployer.Wpf;
using DynamicData;
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
        private readonly IDeploymentLibrary deploymentLibrary;
        private readonly IDeviceDeployer deviceDeployer;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IInteraction interaction;
        private DeploymentDto deployment;
        private ReadOnlyObservableCollection<DeploymentDto> depls;

        private DeviceDto device;
        private ObservableAsPropertyHelper<IEnumerable<DeviceDto>> devices;
        private ObservableAsPropertyHelper<List<DeploymentDto>> allDeployments;

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

        public DeviceDto Device
        {
            get => device;
            set => this.RaiseAndSetIfChanged(ref device, value);
        }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> Deploy { get; set; }

        public ReadOnlyObservableCollection<DeploymentDto> FilteredDeployments => depls;

        public IEnumerable<DeviceDto> Devices => devices.Value;

        public ReactiveCommand<Unit, List<DeploymentDto>> FetchAllDeployments { get; set; }

        public ReactiveCommand<Unit, List<DeviceDto>> FetchDevices { get; set; }

        public ReactiveCommand<Unit, object> Fetch { get; set; }

        public IObservable<bool> IsBusyObservable { get; }

        private void ConfigureCommands()
        {
            ConfigureDeployCommand();

            FetchDevices = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Devices());
            devices = FetchDevices.ToProperty(this, x => x.Devices);
            FetchAllDeployments = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Deployments());

            var filter = this.WhenAnyValue(x => x.Device).Select(dto => BuildFilter(dto));
            allDeployments = FetchAllDeployments.ToProperty(this, x => x.AllDeployments);

            var filteredData = new SourceCache<DeploymentDto, int>(d => d.Id);

            FetchAllDeployments.Subscribe(dtos => filteredData.AddOrUpdate(dtos));

            Fetch = ReactiveCommand.CreateFromObservable(() => ObservableMixins.Cast<object>(FetchAllDeployments.Execute()).Concat(ObservableMixins.Cast<object>(FetchDevices.Execute())));


            filteredData.Connect()
                .Filter(filter)
                .Bind(out depls)
                .Subscribe()
                .DisposeWith(disposables);
        }

        public List<DeploymentDto> AllDeployments => allDeployments.Value;

        private Func<DeploymentDto, bool> BuildFilter(DeviceDto dto)
        {
            if (dto == null)
            {
                return d => false;
            }
            
            return d =>
            {
                if (d == null)
                {
                    return false;
                }

                return d.Devices.Any(i => dto.Id == i);
            };
        }

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