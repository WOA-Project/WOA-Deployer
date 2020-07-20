using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAll();
    }
}