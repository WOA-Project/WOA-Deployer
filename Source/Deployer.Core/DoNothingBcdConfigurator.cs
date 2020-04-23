using System.Threading.Tasks;
using Deployer.Core.FileSystem;

namespace Deployer.Core
{
    public class DoNothingBcdConfigurator : IBcdConfigurator
    {
        public Task Setup(string bcdPath, IPartition efiEsp)
        {
            return Task.CompletedTask;
        }
    }
}