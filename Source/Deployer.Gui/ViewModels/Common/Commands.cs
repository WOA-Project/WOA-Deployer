using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using Deployer.Core;
using Deployer.Core.Exceptions;
using Deployer.Core.Services.Wim;
using Deployer.Gui.Properties;
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
    }
}