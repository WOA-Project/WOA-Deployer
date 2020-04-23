using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting;
using Grace.DependencyInjection.Attributes;

namespace Deployer.Core
{
    [Metadata("Name", "Do nothing")]
    [Metadata("Order", 0)]
    public class NullLayoutPreparer : IDiskLayoutPreparer
    {
        public Task Prepare(IDisk disk)
        {
            return Task.CompletedTask;
        }
    }
}