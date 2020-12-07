using Deployer.Filesystem;

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