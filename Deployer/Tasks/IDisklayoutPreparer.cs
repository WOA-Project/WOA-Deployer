using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Tasks
{
    public interface IDiskLayoutPreparer
    {
        Task Prepare(Disk disk);
    }    
}