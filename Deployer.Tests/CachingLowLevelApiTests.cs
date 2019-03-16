using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Execution.Testing;
using Deployer.FileSystem;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class CachingLowLevelApiTests
    {
        [Fact]
        public async Task Get()
        {
            var testLowLevelApi = new TestLowLevelApi();
            var caching = new CachingLowLevelApi(testLowLevelApi);
            var disks = await caching.GetDisks();
            disks = await caching.GetDisks();
            testLowLevelApi.CalledMethods.Should().HaveCount(1);
        }
    }    
}