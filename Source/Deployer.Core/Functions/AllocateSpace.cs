using System;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions
{
    public class AllocateSpace : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public AllocateSpace(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(int diskNumber, string gptPartition, int requiredGBs)
        {
            var requiredSize = ByteSize.FromGigaBytes(requiredGBs);
            Log.Verbose("Verifying available space");
            Log.Verbose("Verifying the available space...");
            Log.Verbose("We will need {Size} of free space for Windows", requiredSize);

            var disk = await fileSystem.GetDisk(diskNumber);
            var hasEnoughSpace = disk.HasEnoughSpace(requiredSize);
            if (!hasEnoughSpace)
            {
                Log.Warning("There's not enough space in the disk. We will try to allocate it automatically");
                await Allocate(() => fileSystem.GetDisk(diskNumber), gptPartition, requiredSize);
                Log.Verbose("Space allocated correctly");
            }
            else
            {
                Log.Verbose("We have enough available space to deploy Windows");
            }
        }

        private async Task Allocate(Func<Task<IDisk>> diskFactory, string partitionName, ByteSize requiredSpace)
        {
            Log.Verbose("Trying to shrink Data partition...");

            var disk = await diskFactory();
            var dataPartition = await disk.GetPartitionByName(partitionName);

            if (dataPartition == null)
            {
                Log.Verbose("Data partition doesn't exist. Skipping.");
                throw new SpaceAllocationException();
            }

            var dataVolume = await dataPartition.GetVolume();

            disk = await diskFactory();
            var data = dataVolume.Size;
            var allocated = disk.AllocatedSize;
            var available = disk.AvailableSize;
            var newData = data - (requiredSpace - available);

            Log.Verbose("Total size allocated: {Size}", allocated);
            Log.Verbose("Space available: {Size}", available);
            Log.Verbose("Space needed: {Size}", requiredSpace);
            Log.Verbose("'Data' size: {Size}", data);
            Log.Verbose("Calculated new size for the 'Data' partition: {Size}", newData);

            Log.Verbose("Resizing 'Data' to {Size}", newData);

            await dataVolume.Partition.Resize(newData);

            Log.Verbose("Resize operation completed successfully");

            disk = await diskFactory();
            var isEnoughAlready = disk.HasEnoughSpace(requiredSpace);

            if (!isEnoughAlready)
            {
                throw new SpaceAllocationException();
            }
        }
    }
}
