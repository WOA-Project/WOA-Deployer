using System.Threading.Tasks;

namespace Deployer
{
    public interface ITooling
    {
        Task ToogleDualBoot(bool isEnabled);
    }
}