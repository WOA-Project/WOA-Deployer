using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Zafiro.Core;

namespace Deployer.Core.Services
{
    public interface IImageFlasher
    {
        Task Flash(IDisk disk, string imagePath, IOperationProgress progressObserver = null);
    }
}