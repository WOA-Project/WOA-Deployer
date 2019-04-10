using System.Threading.Tasks;
using Deployer.FileSystem;
using Xunit;

namespace Deployer.NetFx.Tests
{
    public class DiskApiTests
    {
        [Fact]
        public async Task GetVolumeByPartitionName()
        {
            var sut = new DiskApi();
            var disk = await sut.GetDisk(3);
            var vols = await disk.GetVolumeByPartitionName("MainOS");
        }
        

        [Fact]
        public async Task GetDataByLabel()
        {
            var diskApi = new DiskApi();
            var disk = await diskApi.GetDisk(3);
            var partition = await disk.GetPartition("Data");
        }        
    }
}