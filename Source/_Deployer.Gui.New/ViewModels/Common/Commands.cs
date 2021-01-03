using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Deployer.Core.Exceptions;
using Deployer.Core.Services.Wim;
using Deployer.Gui.ViewModels.Common;
using ReactiveUI;
using Serilog;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;

namespace Deployer.Gui.New.ViewModels.Common
{
    public class Commands
    {
        private readonly IOpenFilePicker openFilePicker;
        private readonly IFileSystemOperations fileSystemOperations;

        public Commands(IOpenFilePicker openFilePicker, IFileSystemOperations fileSystemOperations)
        {
            this.openFilePicker = openFilePicker;
            this.fileSystemOperations = fileSystemOperations;
            PickWimFileCommand = ReactiveCommand.CreateFromObservable(() => PickWimFileObs);
        }

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

                openFilePicker.FileTypeFilter = filters.Select(tuple => new FileTypeFilter(tuple.Item1, tuple.Item2.ToArray())).ToList();
                var value = openFilePicker.PickFile();

                return Observable.Return(value).Where(x => x != null)
                    .Select(LoadWimMetadata);
            }
        }
        public ReactiveCommand<Unit, WimMetadataViewModel> PickWimFileCommand { get; }

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