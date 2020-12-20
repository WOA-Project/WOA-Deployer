using System.Threading.Tasks;
using Deployer.Filesystem;

namespace Deployer.Tools.Bcd
{
    public interface IBcdConfigurator
    {
        Task Setup(string bcdPath, IPartition efiEsp);
    }
}