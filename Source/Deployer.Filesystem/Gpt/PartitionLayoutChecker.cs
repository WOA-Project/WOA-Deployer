using System.Collections.Generic;
using System.Linq;
using Zafiro.Core.Mixins;

namespace Deployer.Filesystem.Gpt
{
    public static class PartitionLayoutChecker
    {
        public static bool IsLayoutValid(IList<Partition> partitions, ulong size)
        {
            if (!partitions.Any())
            {
                return true;
            }

            var isOverlapping = partitions.Overlap(x => x.FirstSector, x => x.LastSector);
            var insideDisk = partitions.Max(x => x.FirstSector) < size && partitions.Max(x => x.LastSector) < size;

            return !isOverlapping && insideDisk;
        }
    }
}