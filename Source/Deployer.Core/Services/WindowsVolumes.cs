using Deployer.Core.FileSystem;

namespace Deployer.Core.Services
{
    public class WindowsVolumes
    {
        public WindowsVolumes(IVolume bootVolume, IVolume windowsVolume)
        {
            Boot = bootVolume;
            Windows = windowsVolume;
        }

        public IVolume Boot { get; }
        public IVolume Windows { get; }
    }
}