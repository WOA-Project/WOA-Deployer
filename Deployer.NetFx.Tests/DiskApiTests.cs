using System.Threading.Tasks;
using Deployer.FileSystem;
using FluentAssertions;
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

        [Fact]
        public async Task GetEspVolume()
        {
            var diskApi = new DiskApi();
            var disk = await diskApi.GetDisk(3);
            var partition = await disk.GetPartition("SYSTEM");
            await partition.SetGptType(PartitionType.Esp);
            await partition.Format(FileSystemFormat.Fat32, "System");

            var vol = await disk.GetVolumeByPartitionName("SYSTEM");
            await vol.Mount();
            vol.Should().NotBeNull();
            vol.Root.Should().NotBeNull();
        }
    }
}