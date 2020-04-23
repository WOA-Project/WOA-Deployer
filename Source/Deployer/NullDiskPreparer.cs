using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Tasks;

namespace Deployer
{
    public class NullDiskPreparer : IDiskLayoutPreparer
    {
        public Task Prepare(IDisk disk)
        {
            throw new System.NotImplementedException();
        }
    }
}