using ByteSizeLib;
using Deployer.Core.FileSystem;

namespace Deployer.Gui.ViewModels.Disk
{
    public class DiskViewModel
    {
        private readonly IDisk disk;

        public DiskViewModel(IDisk disk)
        {
            this.disk = disk;
        }

        public uint Number => disk.Number + 1;
        public string FriendlyName => disk.FriendlyName;
        public ByteSize Size => disk.Size;
        public bool IsUsualTarget => Size > ByteSize.FromGigaBytes(1) && Size < ByteSize.FromGigaBytes(200);

        public override string ToString()
        {
            return $"Disk number {disk.Number}, {disk.FriendlyName}, Capacity: {disk.Size}";
        }
    }
}