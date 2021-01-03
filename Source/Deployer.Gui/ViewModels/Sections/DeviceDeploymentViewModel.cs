using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Interaction;
using Deployer.Wpf;
using DynamicData;
using ExtendedXmlSerializer.ExtensionModel.Types;
using Grace.DependencyInjection.Attributes;
using Iridio.Runtime.ReturnValues;
using Optional;
using ReactiveUI;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;
using Zafiro.UI;
using Option = Optional.Option;

namespace Deployer.Gui.ViewModels.Sections
{
    [Metadata("Name", "Main")]
    [Metadata("Order", 1)]
    public class DeviceDeploymentViewModel : ReactiveObject, ISection
    {
        private const string FeedFolder = "Feed";
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");
        private readonly IWoaDeployer deployer;
        private readonly IDeploymentLibrary deploymentLibrary;
        private readonly IDeviceDeployer deviceDeployer;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IInteraction interaction;
        private DeploymentDto deployment;
        private ReadOnlyObservableCollection<DeploymentDto> deployments;
        private DeviceDto device;
        private ObservableAsPropertyHelper<IEnumerable<DeviceDto>> devices;
        private readonly ObservableAsPropertyHelper<bool> isBusy;

        public DeviceDeploymentViewModel(IDeviceDeployer deviceDeployer, IInteraction interaction,
            OperationProgressViewModel operationProgress, IDeploymentLibrary deploymentLibrary,
            IFileSystemOperations fileSystemOperations, IWoaDeployer deployer)
        {
            OperationProgress = operationProgress;
            this.deviceDeployer = deviceDeployer;
            this.interaction = interaction;
            this.deploymentLibrary = deploymentLibrary;
            this.fileSystemOperations = fileSystemOperations;
            this.deployer = deployer;

            ConfigureCommands();

            this.WhenAnyValue(x => x.Device)
                .InvokeCommand(SelectFirstDeployment);

            DownloadFeedCommand = ReactiveCommand.CreateFromTask(DownloadFeed);

            isBusy = IsBusyObservable.ToProperty(this, x => x.IsBusy);
        }

        public bool IsBusy => isBusy.Value;

        public ReactiveCommand<Unit, Either<DeployerError, Success>> DownloadFeedCommand { get; }

        public ReactiveCommand<DeviceDto, Unit> SelectFirstDeployment { get; set; }

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

        public ReadOnlyObservableCollection<DeploymentDto> FilteredDeployments => deployments;

        public IEnumerable<DeviceDto> Devices => devices.Value;

        public ReactiveCommand<Unit, List<DeploymentDto>> FetchDeployments { get; set; }

        public ReactiveCommand<Unit, List<DeviceDto>> FetchDevices { get; set; }

        public ReactiveCommand<Unit, Either<DeployerError, List<DeploymentDto>>> Refresh { get; set; }

        public IObservable<bool> IsBusyObservable => Refresh.IsExecuting.Merge(Deploy.IsExecuting);

        private async Task DeleteFeedFolder()
        {
            if (fileSystemOperations.DirectoryExists(FeedFolder))
            {
                await fileSystemOperations.DeleteDirectory(FeedFolder);
            }
        }

        private async Task<Either<DeployerError, Success>> DownloadFeed()
        {
            await DeleteFeedFolder();
            return await Run(BootstrapPath);
        }

        private Task<Either<DeployerError, Success>> Run(string bootstrapPath)
        {
            return deployer.Run(bootstrapPath);
        }

        private void ConfigureCommands()
        {
            ConfigureDeployCommand();

            FetchDevices = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Devices());
            devices = FetchDevices.ToProperty(this, x => x.Devices);
            FetchDeployments = ReactiveCommand.CreateFromTask(() => deploymentLibrary.Deployments());

            var filter = this.WhenAnyValue(x => x.Device).Select(dto => BuildFilter(dto));

            FetchDeployments.SelectMany(x => x)
                .ToObservableChangeSet(s => s.Id)
                .Filter(filter)
                .Bind(out deployments)
                .Subscribe()
                .DisposeWith(disposables);

            SelectFirstDeployment = ReactiveCommand.Create<DeviceDto, Unit>(_ =>
            {
                if (deployments.Count == 1) Deployment = deployments.First();

                return Unit.Default;
            });

            Refresh = ReactiveCommand.CreateFromTask(async () =>
            {
                return await (await DownloadFeedCommand.Execute())
                    .MapRight(async success => await FetchDevices.Execute())
                    .MapRight(async task => await FetchDeployments.Execute()).RightTask();
            });

            Refresh.SelectMany(either =>
                {
                    var mapRight = either
                        .MapRight(error => Task.FromResult(Unit.Default))
                        .Handle(async error =>
                        {
                            await RefreshWentWrong(error);
                            return Unit.Default;
                        });

                    return mapRight;
                })
                .Subscribe()
                .DisposeWith(disposables);
        }

        private Task RefreshWentWrong(DeployerError deployerError)
        {
            return interaction.Message("Error", $"Could not fetch the bootstrap files: {deployerError}",
                "OK".Some(), Option.None<string>());
        }


        private Task DeploymentWentWrong(DeployerError deployerError)
        {
            return interaction.Message("Error", $"The deployment has failed: {deployerError}",
                "OK".Some(), Option.None<string>());
        }

        private Task DeploymentWasOk()
        {
            return interaction.Message("Done", "The deployment has finished successfully", "OK".Some(),
                Option.None<string>());
        }

        private Func<DeploymentDto, bool> BuildFilter(DeviceDto dto)
        {
            if (dto == null) return d => false;

            return d =>
            {
                if (d == null) return false;

                return d.Devices.Any(i => dto.Id == i);
            };
        }

        private void ConfigureDeployCommand()
        {
            var hasDevice = this.WhenAnyValue(model => model.Deployment).Select(d => d != null);
            Deploy = ReactiveCommand.CreateFromTask(
                () => deviceDeployer.Deploy(new DeploymentRequest(Device, deployment.ScriptPath)),
                hasDevice);


            Deploy.SelectMany(either =>
                {
                    var mapRight = either
                        .MapRight(async error =>
                        {
                            await DeploymentWasOk();
                            return Unit.Default;
                        })
                        .Handle(async error =>
                        {
                            await DeploymentWentWrong(error);
                            return Unit.Default;
                        });

                    return mapRight;
                })
                .Subscribe()
                .DisposeWith(disposables);
        }
    }
}