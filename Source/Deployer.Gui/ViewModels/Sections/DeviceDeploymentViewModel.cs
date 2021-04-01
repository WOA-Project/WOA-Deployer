using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Interaction;
using Deployer.Wpf;
using DynamicData;
using Grace.DependencyInjection.Attributes;
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

#if DEBUG
        private const string BoostrapFilename = "Bootstrap.Debug.txt";
#else
        private const string BoostrapFilename = "Bootstrap.Release.txt";
#endif
        private const string FeedFolder = "Feed";
        private static readonly string BootstrapPath = Path.Combine("Core", BoostrapFilename);
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
        private ObservableAsPropertyHelper<bool> isRefreshVisible;

        public DeviceDeploymentViewModel(IDeviceDeployer deviceDeployer, IInteraction interaction,
            OperationStatusViewModel operationStatus, IDeploymentLibrary deploymentLibrary,
            IFileSystemOperations fileSystemOperations, IWoaDeployer deployer)
        {
            OperationStatus = operationStatus;
            this.deviceDeployer = deviceDeployer;
            this.interaction = interaction;
            this.deploymentLibrary = deploymentLibrary;
            this.fileSystemOperations = fileSystemOperations;
            this.deployer = deployer;

            ConfigureCommands();
            
            this.WhenAnyValue(x => x.Device)
                .InvokeCommand(SelectFirstDeployment);

            DownloadFeedCommand = ReactiveCommand.CreateFromObservable(DownloadFeed);

            isBusy = IsBusyObservable.ToProperty(this, x => x.IsBusy);
        }

        public bool IsBusy => isBusy.Value;

        public ReactiveCommand<Unit, Either<GenericError, GenericSuccess>> DownloadFeedCommand { get; }

        public ReactiveCommand<DeviceDto, Unit> SelectFirstDeployment { get; set; }

        public OperationStatusViewModel OperationStatus { get; }

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

        public ReactiveCommand<Unit, Either<DeployerError, DeploymentSuccess>> Deploy { get; set; }

        public ReadOnlyObservableCollection<DeploymentDto> FilteredDeployments => deployments;

        public IEnumerable<DeviceDto> Devices => devices.Value;

        public ReactiveCommand<Unit, List<DeploymentDto>> FetchDeployments { get; set; }

        public ReactiveCommand<Unit, List<DeviceDto>> FetchDevices { get; set; }

        public ReactiveCommand<Unit, Either<GenericError, List<DeploymentDto>>> Refresh { get; set; }

        public IObservable<bool> IsBusyObservable => Refresh.IsExecuting.Merge(Deploy.IsExecuting);

        private async Task<Either<GenericError, GenericSuccess>> DeleteFeedFolder()
        {
            if (fileSystemOperations.DirectoryExists(FeedFolder))
            {
                try
                {
                    await fileSystemOperations.DeleteDirectory(FeedFolder);
                }
                catch (Exception e)
                {
                    return new GenericError(e.Message);
                }
            }

            return new GenericSuccess();
        }

        private IObservable<Either<GenericError, GenericSuccess>> DownloadFeed()
        {
            var downloadFeed = Observable.FromAsync(DeleteFeedFolder)
                .SelectMany(async dl =>
                {
                    var run = await Run(BootstrapPath);
                    var dlAndRun = dl.MapRight(_ => run);
                    return dlAndRun;
                });

            return downloadFeed;
        }

        private async Task<Either<GenericError, GenericSuccess>> Run(string bootstrapPath)
        {
            var task = await deployer.Run(bootstrapPath);
            return task
                .MapLeft(s => new GenericError(s.ToString()))
                .MapRight(s => new GenericSuccess());
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

            isRefreshVisible = this
                .WhenAnyValue(x => x.Devices)
                .Select(x => x == null || !x.Any())
                .ToProperty(this, d => d.IsRefreshVisible);
        }

        public bool IsRefreshVisible => isRefreshVisible.Value;

        private Task RefreshWentWrong(GenericError error)
        {
            return interaction.Message("Error", $"Could not fetch the bootstrap files: {error}",
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

    public class GenericSuccess
    {
    }

    public class GenericError
    {
        public string Message { get; }

        public GenericError(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}