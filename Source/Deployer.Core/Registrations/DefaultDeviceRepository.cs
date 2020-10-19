using System.Threading.Tasks;

namespace Deployer.Core.Registrations
{
    public class DefaultDeviceRepository : IDevRepo
    {
        public Task<DeployerStore> Get()
        {
            return Task.FromResult(DefaultStore.GetDeployerStore());
        }
    }
}