namespace Deployer.Gui
{
    public interface ISettingsService
    {
        string WimFolder { get; set; }
        double SizeReservedForWindows { get; set; }
        bool UseCompactDeployment { get; set; }
        void Save();
    }
}