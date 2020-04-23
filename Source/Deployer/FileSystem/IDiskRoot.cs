using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.FileSystem
{
    public interface IDiskRoot
    {
        Task<IList<IDisk>> GetDisks();
        Task<IDisk> GetDisk(int n);
    }
}