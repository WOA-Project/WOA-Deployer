using System.Threading.Tasks;
using Deployer.Core.FileSystem;

namespace Deployer.Core.Services
{
    public interface IImageFlasher
    {
        Task Flash(IDisk disk, string imagePath, IOperationProgress progressObserver = null);
    }
}