using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Filesystem
{
    public interface IFileSystem
    {
        Task<IList<IDisk>> GetDisks();
        Task<IDisk> GetDisk(int n);
    }
}