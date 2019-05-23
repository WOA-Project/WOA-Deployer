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
            var root = new DiskRoot();
            var disk = await root.GetDisk(3);
            //var vols = await disk.GetVolumeByPartitionName("MainOS");
        }
        

        [Fact]
        public async Task GetDataByLabel()
        {
            var root = new DiskRoot();
            var disk = await root.GetDisk(3);
            //var partition = await disk.GetPartition("Data");
        }        
    }
}