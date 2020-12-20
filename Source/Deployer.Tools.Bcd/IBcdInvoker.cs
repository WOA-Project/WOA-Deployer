using System.Threading.Tasks;

namespace Deployer.Tools.Bcd
{
    public interface IBcdInvoker
    {
        Task<string> Invoke(string command = "");
    }
}