using System.Threading.Tasks;

namespace Deployer.Core.DeploymentLibrary
{
    public interface IDevRepo
    {
        Task<DeployerStore> Get();
    }
}