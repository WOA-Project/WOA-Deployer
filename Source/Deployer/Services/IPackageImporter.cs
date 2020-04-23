using System;
using System.Threading.Tasks;

namespace Deployer.Services
{
    public interface IPackageImporter
    {
        Task Extract(string packagePath, IOperationProgress progressObserver = null);
        Task<string> GetReadmeText(string fileName);
    }
}