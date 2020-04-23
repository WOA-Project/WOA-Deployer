using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core.FileSystem
{
    public interface IFileSystem
    {
        Task<IList<IDisk>> GetDisks();
        Task<IDisk> GetDisk(int n);
    }
}