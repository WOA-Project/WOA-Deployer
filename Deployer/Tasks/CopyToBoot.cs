using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.FileSystem;

namespace Deployer.Tasks
{
    [TaskDescription("Copying file to BOOT: {0} to {1}")]
    public class CopyToBoot : IDeploymentTask
    {
        private readonly string origin;
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDeviceProvider deviceProvider;

        public CopyToBoot(string origin, string destination, IFileSystemOperations fileSystemOperations, IDeviceProvider deviceProvider)
        {
            this.origin = origin;
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
            this.deviceProvider = deviceProvider;
        }

        public async Task Execute()
        {
            var device = deviceProvider.Device;

            var disk = await device.GetDeviceDisk();
            var espPart = await disk.GetBootEfiEspPartition();
            if (espPart != null)
            {
                await espPart.SetGptType(PartitionType.Basic);
            }

            var bootVol = await device.GetBootVolume();

            var finalPath = Path.Combine(bootVol.RootDir.Name, destination);
            await fileSystemOperations.Copy(origin, finalPath);

            await bootVol.Partition.SetGptType(PartitionType.Esp);
        }
    }
}