using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Core.DeploymentLibrary;

namespace Deployer.Core
{
    public interface IContextualizer
    {
        Task Setup(IDictionary<string, object> variables);
        bool CanContextualize(Device device);
    }
}