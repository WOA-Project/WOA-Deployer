using Deployer.FileSystem.Gpt;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class SizeAssignerTests
    {
        [Theory]
        [InlineData(0, 512, 0, 511)]
        [InlineData(3, 512, 0, 511)]
        [InlineData(512, 512, 512, 1023)]
        [InlineData(0, 1536, 0, 1023)]
        [InlineData(18151232, 18151232 + 1024, 18151232, 18151232 + 1024)]
        public void TestSize(ulong start, ulong length, ulong expectedStart, ulong expectedEnd)
        {
            var calculator = new PartitionSegmentCalculator(512, 1536);
            var partSize = calculator.Constraint(new GptSegment(start, length));
            partSize.Should().Be(new GptSegment(expectedStart, expectedEnd));
        }    
        
        [Theory]
        [InlineData(18151231, 18151424)]
        public void TestNextSector(ulong last, ulong next)
        {
            last.Should().BeLessThan(next);

            var calculator = new PartitionSegmentCalculator(256, ulong.MaxValue);
            calculator.GetNextSector(last).Should().Be(next);
        }   
    }
}