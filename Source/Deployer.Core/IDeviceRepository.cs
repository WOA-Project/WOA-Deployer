using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IDevRepo
    {
        Task<DeployerStore> Get();
    }
}