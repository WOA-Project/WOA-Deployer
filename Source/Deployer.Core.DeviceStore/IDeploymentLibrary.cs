using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core.DeploymentLibrary
{
    public interface IDeploymentLibrary
    {
        Task<List<DeviceDto>> Devices();
        Task<List<DeploymentDto>> Deployments();
    }
}