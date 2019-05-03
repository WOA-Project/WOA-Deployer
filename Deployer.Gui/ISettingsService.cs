using Deployer.Tasks;

namespace Deployer.Gui
{
    public interface ISettingsService
    {
        string WimFolder { get; set; }
        double SizeReservedForWindows { get; set; }
        bool UseCompactDeployment { get; set; }
        bool CleanDownloadedBeforeDeployment { get; set; }
        IDiskLayoutPreparer DiskPreparer { get; set; }
        void Save();
    }
}