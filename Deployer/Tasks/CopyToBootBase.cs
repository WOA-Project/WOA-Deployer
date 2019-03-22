using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.FileSystem;

namespace Deployer.Tasks
{
    [TaskDescription("Copying file to BOOT: {0} to {1}")]
    public abstract class CopyToBootBase : IDeploymentTask
    {
        private readonly string origin;
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDeviceProvider deviceProvider;

        public CopyToBootBase(string origin, string destination, IFileSystemOperations fileSystemOperations, IDeviceProvider deviceProvider) 
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
            var espPart = await disk.GetPartition(SystemPartitionName);
            if (espPart != null)
            {
                await espPart.SetGptType(PartitionType.Basic);
            }

            var bootVol = await device.GetSystemVolume();

            if (bootVol == null)
            {
                throw new ApplicationException("Could not find the System partition. Is Windows 10 ARM64 installed?");
            }

            var finalPath = Path.Combine(bootVol.Root, destination);
            await fileSystemOperations.Copy(origin, finalPath);

            await bootVol.Partition.SetGptType(PartitionType.Esp);
        }

        public abstract string SystemPartitionName { get; }
    }

    
}