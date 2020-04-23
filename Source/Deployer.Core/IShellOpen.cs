using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IShellOpen
    {
        Task Open(string filename);
    }
}