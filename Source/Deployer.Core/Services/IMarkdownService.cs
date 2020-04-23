using System.Threading.Tasks;

namespace Deployer.Core.Services
{
    public interface IMarkdownService
    {
        Task FromFile(string path);
        Task Show(string markdown);
    }
}