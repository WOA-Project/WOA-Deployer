using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Services;
using Deployer.Filesystem;
using Grace.DependencyInjection;

namespace Deployer.NetFx.Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void Dependencies_are_fulfilled()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(block =>
            {
                FunctionDependencies.Configure(block);
                block.Export<FileSystem>().As<IFileSystem>();
                block.Export<TestMarkdownService>().As<IMarkdownService>();
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
