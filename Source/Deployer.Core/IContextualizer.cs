using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IContextualizer
    {
        Task Setup(IDictionary<string, object> variables);
        bool CanContextualize(Device device);
    }
}