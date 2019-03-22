using System.Threading.Tasks;
using Deployer.FileSystem;
using Xunit;

namespace Deployer.NetFx.Tests
{
    public class DiskApiTests
    {
        [Fact]
        public async Task GetVolumeByName()
        {
            var sut = new DiskApi();
            var disk = await sut.GetDisk(3);
            var vols = await disk.GetVolumeByLabel("MainOS");
        }
    }
}