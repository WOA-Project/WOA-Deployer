using System.Threading.Tasks;

namespace Deployer.Core
{
    public interface IInstanceBuilderProvider
    {
        Task<IInstanceBuilder> Create();
    }
}