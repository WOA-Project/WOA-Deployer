namespace Deployer.Gui.ViewModels.Common
{
    public class WimImage
    {
        public WimImage(string path, int index)
        {
            Path = path;
            Index = index;
        }

        public string Path { get; }
        public int Index { get; }
    }
}