using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Cli.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IShellOpen shellOpen;

        public MarkdownService(IFileSystemOperations fileSystemOperations, IShellOpen shellOpen)
        {
            this.fileSystemOperations = fileSystemOperations;
            this.shellOpen = shellOpen;
        }

        public Task FromFile(string path)
        {
            return shellOpen.Open(path);
        }

        public Task Show(string markdown)
        {
            var path = fileSystemOperations.GetTempFileName() + ".md";
            fileSystemOperations.WriteAllText(path, markdown);
            return shellOpen.Open(path);
        }
    }
}