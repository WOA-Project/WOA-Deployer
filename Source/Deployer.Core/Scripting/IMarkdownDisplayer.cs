using System.Threading.Tasks;

namespace Deployer.Core.Scripting
{
    public interface IMarkdownDisplayer
    {
        Task Display(string title, string message);
    }
}