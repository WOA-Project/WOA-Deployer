using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Deployer.Core.Interaction;
using Microsoft.Win32;
using Zafiro.Core.FileSystem;

namespace Deployer.UI
{
    public class OpenFilePicker : IOpenFilePicker
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public OpenFilePicker(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public string InitialDirectory { get; set; }
        public List<FileTypeFilter> FileTypeFilter { get; } = new List<FileTypeFilter>();
        public string PickFile()
        {
            var dialog = new OpenFileDialog();
            var lines = FileTypeFilter.Select(x =>
            {
                var exts = string.Join(";", x.Extensions);
                return $"{x.Description}|{exts}";
            });

            var filter = string.Join("|", lines);

            dialog.Filter = filter;
            dialog.FileName = "";

            if (fileSystemOperations.DirectoryExists(InitialDirectory))
            {
                dialog.InitialDirectory = InitialDirectory;
            }
            
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return dialog.FileName;
            }

            return null;
        }       
    }
}