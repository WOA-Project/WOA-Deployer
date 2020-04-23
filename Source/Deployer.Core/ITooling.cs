using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface ITooling
    {
        Task ToogleDualBoot(bool isEnabled);
    }
}