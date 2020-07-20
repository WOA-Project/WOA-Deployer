using System.Threading.Tasks;
using Zafiro.Core;

namespace Deployer.Core.Services
{
    public interface IArchiveUncompressor
    {
        Task Extract(string archivePath, string destination, IOperationProgress progressObserver = null);
        Task<string> ReadToEnd(string archivePath, string key);
    }
}