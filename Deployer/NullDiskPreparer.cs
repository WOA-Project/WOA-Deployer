using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Tasks;

namespace Deployer
{
    public class NullDiskPreparer : IDiskLayoutPreparer
    {
        public Task Prepare(Disk disk)
        {
            throw new System.NotImplementedException();
        }
    }
}