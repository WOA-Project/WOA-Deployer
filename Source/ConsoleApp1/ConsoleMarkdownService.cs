using System;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Services;

namespace ConsoleApp1
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
            Console.WriteLine($"Display Markdown from {path}");
            await shellOpen.Open(path);
        }

        public Task Show(string markdown)
        {
            Console.WriteLine($"[TODO] Display Markdown from content");
            return Task.CompletedTask;
        }
    }
}