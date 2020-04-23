using System.Threading.Tasks;
using Deployer.FileSystem;

namespace Deployer
{
    public interface ILogCollector
    {
        Task Collect(IDevice device, string savePath);
    }
}