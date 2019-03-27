using System.Collections.Generic;
using System.Linq;
using Zafiro.Core;

namespace Deployer.FileSystem.Gpt
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
            var insideDisk = partitions.Max(x => x.FirstSector <= size);

            return !isOverlapping && insideDisk;
        }
    }
}