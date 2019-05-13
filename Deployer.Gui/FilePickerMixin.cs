using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace Deployer.UI
{
    public static class FilePickerMixin
    {
        public static string Pick(this IOpenFilePicker openOpenFileService, IEnumerable<(string, IEnumerable<string>)> extensions, Func<string> getCurrentFolder, Action<string> setCurrentFolder)
        {
            var fileTypeFilters = extensions.Select(tuple => new FileTypeFilter(tuple.Item1, tuple.Item2.ToArray()));

            openOpenFileService.FileTypeFilter.Clear();
            openOpenFileService.FileTypeFilter.AddRange(fileTypeFilters);

            openOpenFileService.InitialDirectory = getCurrentFolder();

            var selected = openOpenFileService.PickFile();

            if (selected != null)
            {
                var directoryName = Path.GetDirectoryName(selected);
                setCurrentFolder(directoryName);
                Log.Verbose("Default directory for WimFolder has been set to {Folder}", directoryName);
            }

            return selected;
        }
    }
}