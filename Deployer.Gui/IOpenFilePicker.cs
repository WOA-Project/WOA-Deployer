using System.Collections.Generic;

namespace Deployer.Gui
{
    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }
}