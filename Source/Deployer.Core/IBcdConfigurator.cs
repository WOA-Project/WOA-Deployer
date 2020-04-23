using System.Threading.Tasks;
using Deployer.Core.FileSystem;

namespace Deployer.Core
{
    public interface IBcdConfigurator
    {
        Task Setup(string bcdPath, IPartition efiEsp);
    }
}