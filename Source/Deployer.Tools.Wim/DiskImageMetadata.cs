namespace Deployer.Tools.Wim
{
    public class DiskImageMetadata
    {
        public int Index { get; set; }
        public string DisplayName { get; set; }
        public MyArchitecture Architecture { get; set; }
        public string Build { get; set; }
    }
}