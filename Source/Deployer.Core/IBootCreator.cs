using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IBootCreator
    {
        Task MakeBootable(string systemRoot, string windowsPath);
    }
}