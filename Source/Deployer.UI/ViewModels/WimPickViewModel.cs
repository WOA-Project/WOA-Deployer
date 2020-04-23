using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Deployer.Core;
using Deployer.Core.Exceptions;
using Deployer.Core.Services.Wim;
using Deployer.UI.Properties;
using ReactiveUI;
using Serilog;
using Zafiro.Core.UI;

namespace Deployer.UI.ViewModels
{
    public class WimPickViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> hasWimHelper;

        private readonly ObservableAsPropertyHelper<WimMetadataViewModel> pickWimFileObs;
        private readonly ISettingsService settingsService;
        private readonly UIServices uiServices;

        public WimPickViewModel(UIServices uiServices, ISettingsService settingsService)
        {
            this.uiServices = uiServices;
            this.settingsService = settingsService;
            var dialog = uiServices.DialogFactory(this);

            PickWimFileCommand = ReactiveCommand.CreateFromObservable(() => PickWimFileObs);
            pickWimFileObs = PickWimFileCommand.ToProperty(this, x => x.WimMetadata);
            dialog.HandleExceptionsFromCommand(PickWimFileCommand, "WIM file error");
            hasWimHelper = this.WhenAnyValue(model => model.WimMetadata, (WimMetadataViewModel x) => x != null)
                .ToProperty(this, x => x.HasWim);

            OpenGetWoaCommand = ReactiveCommand.Create((string url) => { Process.Start(url); });
        }

        public ReactiveCommand<Unit, WimMetadataViewModel> PickWimFileCommand { get; set; }

        public WimMetadataViewModel WimMetadata => pickWimFileObs.Value;

        private IObservable<WimMetadataViewModel> PickWimFileObs
        {
            get
            {
                var filters = new List<(string, IEnumerable<string>)>
                {
                    ("install.wim", new[]
                    {
                        "install.wim",
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

                var value = uiServices.OpenFilePicker.Pick(filters, () => settingsService.WimFolder, x =>
                {
                    settingsService.WimFolder = x;
                });

                return Observable.Return(value).Where(x => x != null)
                    .Select(LoadWimMetadata);
            }
        }

        public bool HasWim => hasWimHelper.Value;

        public IReactiveCommand OpenGetWoaCommand { get; }

        private static WimMetadataViewModel LoadWimMetadata(string path)
        {
            Log.Verbose("Trying to load WIM metadata file at '{ImagePath}'", path);

            using (var file = File.OpenRead(path))
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
    }
}