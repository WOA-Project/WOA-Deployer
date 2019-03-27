using System.Collections.Generic;
using Deployer.FileSystem;
using Deployer.FileSystem.Gpt;
using FluentAssertions;
using Xunit;
using Partition = Deployer.FileSystem.Gpt.Partition;

namespace Deployer.Tests
{
    public class PartitionLayoutCheckerTests
    {
        [Fact]
        public void ZeroPartitions()
        {
            PartitionLayoutChecker.IsLayoutValid(new List<Partition>(), 8092)
                .Should().Be(true);
        }

        [Fact]
        public void PartitionOutOfDisk()
        {
            var partitions = new List<Partition>()
            {
                CreatePartition(0, 1023),
                CreatePartition(1024, 10200),                
            };

            PartitionLayoutChecker.IsLayoutValid(partitions, 8092)
                .Should().Be(false);
        }

        [Fact]
        public void PartitionOutOfDiskPartitionEndingInSizeSector()
        {
            var partitions = new List<Partition>()
            {
                CreatePartition(0, 1023),
                CreatePartition(1024, 8092),                
            };

            PartitionLayoutChecker.IsLayoutValid(partitions, 8092)
                .Should().Be(false);
        }

        [Fact]
        public void PartitionOutOfDiskPartitionEndingInSizeSectorLessOne()
        {
            var partitions = new List<Partition>()
            {
                CreatePartition(0, 1023),
                CreatePartition(1024, 8092),                
            };

            PartitionLayoutChecker.IsLayoutValid(partitions, 8091)
                .Should().Be(true);
        }

        [Fact]
        public void GoodLayout()
        {
            var partitions = new List<Partition>()
            {
                CreatePartition(0, 1023),
                CreatePartition(1024, 2048),                
            };

            PartitionLayoutChecker.IsLayoutValid(partitions, 8092)
                .Should().Be(true);
        }

        [Fact]
        public void OverlappingPartitions()
        {
            var partitions = new List<Partition>()
            {
                CreatePartition(0, 1023),
                CreatePartition(512, 1023),                
            };

            PartitionLayoutChecker.IsLayoutValid(partitions, 8092)
                .Should().Be(false);
        }

        private static Partition CreatePartition(ulong start, ulong end)
        {
            return new Partition("", PartitionType.Basic, 0, 512)
            {
                FirstSector = start,
                LastSector = end,
            };
        }
    }
}