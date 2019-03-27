using System.Collections.Generic;
using Deployer.FileSystem.Gpt;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class PartitionLayoutCheckerTests
    {
        [Fact]
        public void ZeroPartitions()
        {
            PartitionLayoutChecker.IsLayoutValid(new List<Partition>(), 0).Should().Be(true);
        }
    }
}