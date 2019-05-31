using System.Threading.Tasks;

namespace Deployer.FileSystem
{
    public static class PartitionMixin
    {
        public static async Task<IPartition> EnsureWritable(this IPartition partition)
        {
            if (partition.Root == null)
            {
                await partition.AssignDriveLetter();
            }

            return partition;
        }
    }
}