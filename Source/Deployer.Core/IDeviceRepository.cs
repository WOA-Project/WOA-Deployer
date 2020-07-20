using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IDeviceRepository
    {
        Task<DeployerStore> Get();
    }
}