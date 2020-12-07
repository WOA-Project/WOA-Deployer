using System;

namespace Deployer.Filesystem.Gpt
{
    public class PartitionSegmentCalculator
    {
        private readonly uint chunkSize;
        private readonly ulong totalSectors;

        public PartitionSegmentCalculator(uint chunkSize, ulong totalSectors)
        {
            this.chunkSize = chunkSize;
            this.totalSectors = totalSectors;
        }

        public GptSegment Constraint(GptSegment desired)
        {
            var size = SmallerDivisible(desired.Length, chunkSize);

            var rightLimit = totalSectors - 1 * chunkSize;

            var finalFirst = Math.Min(SmallerDivisible(desired.Start, chunkSize), rightLimit);
            var finalLast = Math.Min(finalFirst + size, rightLimit);
            var finalLength = finalLast - finalFirst;
            return new GptSegment(finalFirst, finalLength);
        }

        private static ulong SmallerDivisible(ulong x, ulong y)
        {
            return x / y * y;
        }

        public ulong GetNextSector(ulong lastSector)
        {
            if (lastSector == 0)
            {
                return 0;
            }

            var boundary = SmallerDivisible(lastSector + 1, chunkSize);
            return boundary + chunkSize;
        }
    }
}