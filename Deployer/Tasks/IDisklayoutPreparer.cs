using System.Threading.Tasks;
using Deployer.FileSystem;
using Grace.DependencyInjection.Attributes;

namespace Deployer.Tasks
{
    public interface IDiskLayoutPreparer
    {
        Task Prepare(Disk disk);
    }    
}