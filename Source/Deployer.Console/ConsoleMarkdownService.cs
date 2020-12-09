using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Services;

namespace Deployer.Console
{
    internal class ConsoleMarkdownService : IMarkdownService
    {
        private readonly IShellOpen shellOpen;

        public ConsoleMarkdownService(IShellOpen shellOpen)
        {
            this.shellOpen = shellOpen;
        }

        public async Task FromFile(string path)
        {
            System.Console.WriteLine($"Display Markdown from {path}");
            await shellOpen.Open(path);
        }

        public Task Show(string markdown)
        {
            System.Console.WriteLine(markdown);
            return Task.CompletedTask;
        }
    }
}