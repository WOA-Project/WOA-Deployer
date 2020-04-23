using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer.Services
{
    public interface IImageFlasher
    {
        Task Flash(IDisk disk, string imagePath, IOperationProgress progressObserver = null);
    }
}