using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Console;
using Deployer.Core.FileSystem;
using Deployer.Core.Services;
using Grace.DependencyInjection;

namespace Deployer.NetFx.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(block =>
            {
                Functions.Configure(block, () => new TestMarkdownService());
            });
            
            foreach (var type in Function.Types)
            {
                container.Locate(type);
            }
        }
    }

    public class TestMarkdownService : IMarkdownService
    {
        public Task FromFile(string path)
        {
            throw new NotImplementedException();
        }

        public Task Show(string markdown)
        {
            throw new NotImplementedException();
        }
    }
}
