using System.Threading.Tasks;
using Deployer.Tasks;

namespace Deployer.Console
{
    public class ConsoleMarkdownDisplayer : IMarkdownDisplayer
    {
        public Task Display(string title, string message)
        {
            System.Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}