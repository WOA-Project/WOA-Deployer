using System.Threading.Tasks;
using Deployer.Filesystem;

namespace Deployer.Core
{
    public interface IBcdConfigurator
    {
        Task Setup(string bcdPath, IPartition efiEsp);
    }
}