using System;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Deployer.Functions.Partitions;
using Optional.Unsafe;
using Serilog;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class AllocateSpace : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public AllocateSpace(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(string partitionDescriptor, double requiredGBs)
        {
            var requiredSize = ByteSize.FromGigaBytes(requiredGBs);
            Log.Verbose("Verifying the available space");
            Log.Verbose("We will need {Size} of free space", requiredSize);

            var optionalPartition = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            var partition = optionalPartition.ValueOrFailure($"Cannot get partition {partitionDescriptor}");
            var hasEnoughSpace = partition.Disk.HasEnoughSpace(requiredSize);
            if (!hasEnoughSpace)
            {
                Log.Warning("There's not enough space in the disk. We will try to allocate it automatically");
                await Allocate(partition, requiredSize);
                Log.Information("Space allocated correctly");
            }
            else
            {
                Log.Information("We have enough available space");
            }
        }

        private async Task Allocate(IPartition partition, ByteSize requiredSpace)
        {
            Log.Information("Trying to shrink {Partition} to {RequiredSpace}...", partition, requiredSpace);

            var dataVolume = await partition.GetVolume();

            var data = dataVolume.Size;
            var allocated = partition.Disk.AllocatedSize;
            var available = partition.Disk.AvailableSize;
            var newSize = data - (requiredSpace - available);

            Log.Verbose("Total size allocated: {Size}", allocated);
            Log.Verbose("Space available: {Size}", available);
            Log.Verbose("Space needed: {Size}", requiredSpace);
            Log.Verbose("'Data' size: {Size}", data);
            Log.Verbose("Calculated new size for the 'Data' partition: {Size}", newSize);

            Log.Verbose("Resizing 'Data' to {Size}", newSize);

            await dataVolume.Partition.Resize(newSize);

            Log.Information("Resize operation completed successfully");

            var refreshedDish= await fileSystem.GetDisk((int)partition.Disk.Number);
            var isEnoughAlready = refreshedDish.HasEnoughSpace(requiredSpace);

            if (!isEnoughAlready)
            {
                throw new SpaceAllocationException();
            }
        }
    }
}
