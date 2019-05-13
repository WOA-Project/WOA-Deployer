using System.Collections.Generic;

namespace Deployer.UI
{
    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }
}