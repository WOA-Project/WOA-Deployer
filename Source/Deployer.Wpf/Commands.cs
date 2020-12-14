using System.Reactive;
using Deployer.Core;
using ReactiveUI;
using Zafiro.Core.FileSystem;
using Zafiro.UI;

namespace Deployer.Wpf
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