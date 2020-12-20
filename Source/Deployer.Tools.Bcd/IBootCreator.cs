using System.Threading.Tasks;

namespace Deployer.Tools.Bcd
{
    public interface IBootCreator
    {
        Task MakeBootable(string systemRoot, string windowsPath);
    }
}