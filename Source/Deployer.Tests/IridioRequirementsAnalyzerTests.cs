using System.Linq;
using Deployer.Core.Requirements;
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
            var expectations = new MissingRequirement[] {new MissingRequirement("Disk1", RequirementDefinition.Disk, "Disco 1"), new MissingRequirement("Disk2", RequirementDefinition.Disk, "Disco 2")};
            reqs.Should().BeEquivalentTo(expectations);
        }
    }
}