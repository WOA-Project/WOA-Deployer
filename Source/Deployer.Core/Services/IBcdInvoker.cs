using System.Threading.Tasks;

namespace Deployer.Core.Services
{
    public interface IBcdInvoker
    {
        Task<string> Invoke(string command = "");
    }
}