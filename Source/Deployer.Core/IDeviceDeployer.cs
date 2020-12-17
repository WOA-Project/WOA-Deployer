using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Iridio.Runtime.ReturnValues;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
{
    public interface IDeviceDeployer
    {
        Task<Either<DeployerError, Success>> Deploy(DeploymentRequest deploymentRequest);
    }
}