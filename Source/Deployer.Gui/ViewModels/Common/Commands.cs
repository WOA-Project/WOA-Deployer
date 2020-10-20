using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Deployer.Core;
using Deployer.Core.Exceptions;
using Deployer.Core.Services.Wim;
using Deployer.Gui.Properties;
using Optional;
using Optional.Unsafe;
using ReactiveUI;
using Serilog;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;

namespace Deployer.Gui.ViewModels.Common
{
    public class Commands
    {
        private readonly ISettingsService settingsService;
        private readonly IOpenFilePicker openFilePicker;
        private readonly IFileSystemOperations fileSystemOperations;

        public Commands(IShellOpen shellOpen, ISettingsService settingsService, IOpenFilePicker openFilePicker, IFileSystemOperations fileSystemOperations)
        {
            this.settingsService = settingsService;
            this.openFilePicker = openFilePicker;
            this.fileSystemOperations = fileSystemOperations;
            ShellOpen = ReactiveCommand.CreateFromTask((string url) => shellOpen.Open(url));
        }

        public ReactiveCommand<string, Unit> ShellOpen { get; }

        public IObservable<WimMetadataViewModel> GetPickWimFileObs(string key)
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

            var value = openFilePicker.Pick(filters, () =>
            {
                var option = settingsService[key];
                var val = option.ValueOrDefault();
                return (string) val;
            }, x => { settingsService[key] = ((object) x).Some(); });

            return Observable.Return(value).Where(x => x != null)
                .Select(LoadWimMetadata);
        }

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


    }
}