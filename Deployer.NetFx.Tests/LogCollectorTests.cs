using System.Threading.Tasks;
using Xunit;

namespace Deployer.NetFx.Tests
{
    public class LogCollectorTests
    {
        [Fact]
        public async Task Collect()
        {
            var collector = new LogCollector(new FileSystemOperations());
            var testDevice = new TestDevice();
            await collector.Collect(testDevice, "logs.zip");
        }
    }
}