using System.Diagnostics;
using System.Reactive;
using Deployer.Core;
using ReactiveUI;
using Zafiro.Core.FileSystem;

namespace Deployer.UI.ViewModels
{
    public class LogViewModel : ReactiveObject
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public LogViewModel(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
            OpenLogFolder = ReactiveCommand.Create(OpenLogs);
        }

        public ReactiveCommand<Unit, Unit> OpenLogFolder { get; set; }

        private void OpenLogs()
        {
            fileSystemOperations.EnsureDirectoryExists("Logs");
            Process.Start("Logs");
        }
    }
}