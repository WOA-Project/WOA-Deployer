using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions
{
    public class DisplayMarkdownMessage : DeployerFunction
    {
        private readonly IMarkdownService markdownService;

        public DisplayMarkdownMessage(IMarkdownService markdownService, 
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.markdownService = markdownService;
        }

        public Task Execute(string markdown)
        {
            OperationContext.CancellationToken.ThrowIfCancellationRequested();

            return markdownService.Show(markdown);
        }
    }
}