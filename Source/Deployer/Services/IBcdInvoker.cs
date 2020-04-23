using System.Threading.Tasks;

namespace Deployer.Services
{
    public interface IBcdInvoker
    {
        Task<string> Invoke(string command = "");
    }
}