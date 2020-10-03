using System.Linq;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class IridioRequirementsAnalyzerTests
    {
        [Fact]
        public void Test()
        {
            var sut = new IridioRequirementsAnalyzer();
            
            var reqs  = sut.GetRequirements("asdfasd //Requires Disk \"Disk1\" as \"Disco 1\"\r\n//Requires Disk \"Disk2\" as \"Disco 2\"");
            var expectations = new FullFilledRequirement[] {new FullFilledRequirement("Disk1", "Disk", "Disco 1"), new FullFilledRequirement("Disk2", "Disk", "Disco 2")};
            reqs.Should().BeEquivalentTo(expectations);
        }
    }
}