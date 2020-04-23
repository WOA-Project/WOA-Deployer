using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Deployer.Core;
using Deployer.Core.Exceptions;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services.Wim;
using Deployer.Gui.Properties;
using ReactiveUI;
using Serilog;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;

namespace Deployer.Gui.ViewModels
{
    public class WimPickViewModel : RequirementFiller
    {
        private readonly ObservableAsPropertyHelper<bool> hasWimHelper;

        private readonly ObservableAsPropertyHelper<WimMetadataViewModel> pickWimFileObs;
        private readonly ISettingsService settingsService;
        private readonly IOpenFilePicker openFilePicker;
        private readonly IFileSystemOperations fileSystemOperations;

        public WimPickViewModel(IDialogService uiServices, ISettingsService settingsService, IOpenFilePicker openFilePicker, IFileSystemOperations fileSystemOperations)
        {
            this.settingsService = settingsService;
            this.openFilePicker = openFilePicker;
            this.fileSystemOperations = fileSystemOperations;

            PickWimFileCommand = ReactiveCommand.CreateFromObservable(() => PickWimFileObs);
            pickWimFileObs = PickWimFileCommand.ToProperty(this, x => x.WimMetadata);

            uiServices.HandleExceptionsFromCommand(PickWimFileCommand);

            hasWimHelper = this.WhenAnyValue(model => model.WimMetadata, (WimMetadataViewModel x) => x != null)
                .ToProperty(this, x => x.HasWim);

            OpenGetWoaCommand = ReactiveCommand.Create((string url) => { Process.Start(url); });

            PickWimFileCommand.Subscribe(x => SetVariables(x));
        }

        private void SetVariables(WimMetadataViewModel wimMetadataViewModel)
        {
            if (wimMetadataViewModel is null)
            {
                Requirements[Requirement.WimFile] = null;
                Requirements[Requirement.WimImageIndex] = null;
                return;
            }

            Requirements[Requirement.WimFile] = wimMetadataViewModel.Path;
            Requirements[Requirement.WimImageIndex] = wimMetadataViewModel.SelectedDiskImage.Index;
        }

        public ReactiveCommand<Unit, WimMetadataViewModel> PickWimFileCommand { get; set; }

        public WimMetadataViewModel WimMetadata => pickWimFileObs.Value;

        private IObservable<WimMetadataViewModel> PickWimFileObs
        {
            get
            {
                var filters = new List<(string, IEnumerable<string>)>
                {
                    ("Windows Images", new[]
                    {
                        "install.wim;install.esd",
                    }),
                    ("Windows Images", new[]
                    {
                        "*.wim",
                        "*.esd"
                    }),
                    ("All files", new[]
                    {
                        "*.*",
                    }),
                };

                var value = openFilePicker.Pick(filters, () => settingsService.WimFolder, x =>
                {
                    settingsService.WimFolder = x;
                });

                return Observable.Return(value).Where(x => x != null)
                    .Select(LoadWimMetadata);
            }
        }

        public bool HasWim => hasWimHelper.Value;

        public IReactiveCommand OpenGetWoaCommand { get; }

        private WimMetadataViewModel LoadWimMetadata(string path)
        {
            Log.Verbose("Trying to load WIM metadata file at '{ImagePath}'", path);

            using (var file = fileSystemOperations.OpenForRead(path))
            {
                var imageReader = new WindowsImageMetadataReader();
                var windowsImageInfo = imageReader.Load(file);
                if (windowsImageInfo.Images.All(x => x.Architecture != Architecture.Arm64))
                {
                    throw new InvalidWimFileException(Resources.WimFileNoValidArchitecture);
                }

                var vm = new WimMetadataViewModel(windowsImageInfo, path);

                Log.Verbose("WIM metadata file at '{ImagePath}' retrieved correctly", path);

                return vm;
            }
        }

        public override IEnumerable<string> HandledRequirements => new[] {Requirement.WimFile, Requirement.WimImageIndex};
    }
}