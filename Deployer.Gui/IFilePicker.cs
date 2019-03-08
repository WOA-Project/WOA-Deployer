using System.Collections.Generic;

namespace Deployer.Gui
{
    public interface IFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }
}