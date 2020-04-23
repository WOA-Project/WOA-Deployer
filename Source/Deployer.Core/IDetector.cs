using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IDetector
    {
        Task<Device> Detect();
    }
}