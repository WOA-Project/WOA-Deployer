using System.Threading.Tasks;
using Deployer.Core.DeploymentLibrary;

namespace Deployer.Core
{
    public interface IDetector
    {
        Task<Device> Detect();
    }
}