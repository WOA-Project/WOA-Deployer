using Xunit;

namespace Deployer.Gui.Tests
{
    public class CompositionTests
    {
        [Fact]
        public void Composition_root_should_be_created_ok()
        {
            var root = new Composition().Root;
        }
    }
}
