using System;
using Deployer.Core.FileSystem.Gpt;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class SizeAssignerTests
    {
        [Theory]
        [InlineData(0, 512, 0, 512)]
        [InlineData(3, 512, 0, 512)]
        [InlineData(512, 512, 512, 512)]
        [InlineData(0, 1536, 0, 1024)]
        [InlineData(0, 1540, 0, 1024)]
        public void TestSize(ulong start, ulong length, ulong expectedStart, ulong expectedLength)
        {
            var calculator = new PartitionSegmentCalculator(512, 1536);
            var partSize = calculator.Constraint(new GptSegment(start, length));
            var gptSegment = new GptSegment(expectedStart, expectedLength);
            partSize.Should().Be(gptSegment);
        }    

        [Fact]
        public void ConstraintToZeroLenght()
        {
            var calculator = new PartitionSegmentCalculator(512, 1536);
            Action t = () => calculator.Constraint(new GptSegment(1536, 1000));
            t.Should().Throw<InvalidOperationException>();
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