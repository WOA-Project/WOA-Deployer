using System.IO;
using System.Threading.Tasks;

namespace Deployer.Core.Scripting
{
    public interface IZipExtractor
    {
        Task Extract(Stream stream, string destination, IOperationProgress progressObserver = null);
    }
}