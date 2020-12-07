using System.Threading.Tasks;
using Deployer.Filesystem;

namespace Deployer.Core.Scripting
{
    public interface IDiskLayoutPreparer
    {
        Task Prepare(IDisk disk);
    }    
}