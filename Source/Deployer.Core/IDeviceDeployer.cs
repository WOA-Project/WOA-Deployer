using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
{
    public interface IDeviceDeployer
    {
        Task<Either<DeployerError, DeploymentSuccess>> Deploy(DeploymentRequest deploymentRequest);
    }
}