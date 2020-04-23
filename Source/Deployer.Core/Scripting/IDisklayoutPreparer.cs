using System.Threading.Tasks;
using Deployer.Core.FileSystem;

namespace Deployer.Core.Scripting
{
    public interface IDiskLayoutPreparer
    {
        Task Prepare(IDisk disk);
    }    
}