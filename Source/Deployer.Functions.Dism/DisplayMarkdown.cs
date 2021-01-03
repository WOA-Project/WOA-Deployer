using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class DisplayMarkdown : DeployerFunction
    {
        private readonly IMarkdownService markdownService;

        public DisplayMarkdown(IMarkdownService markdownService, 
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.markdownService = markdownService;
        }

        public Task Execute(string path)
        {
            return markdownService.FromFile(path);
        }
    }
}