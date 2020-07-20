using System.Threading.Tasks;
using Zafiro.Core;

namespace Deployer.Core.Services
{
    public interface IPackageImporter
    {
        Task Extract(string packagePath, IOperationProgress progressObserver = null);
        Task<string> GetReadmeText(string fileName);
    }
}