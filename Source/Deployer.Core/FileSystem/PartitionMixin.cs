using System.Threading.Tasks;

namespace Deployer.Core.FileSystem
{
    public static class PartitionMixin
    {
        public static async Task<IPartition> EnsureWritable(this IPartition partition)
        {
            if (partition.Root == null)
            {
                await partition.AssignDriveLetter();

                if (partition.Root == null)
                {
                    throw new FileSystemException($"Could not map the partition {partition} to a drive letter");
                }
            }

            return partition;
        }
    }
}