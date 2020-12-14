using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Services;
using Optional;
using Zafiro.Core.FileSystem;
using Zafiro.UI;

namespace Deployer.Wpf
{
    public class MarkdownService : IMarkdownService
    {
        private readonly IInteraction dialogService;
        private readonly IFileSystemOperations fileSystemOperations;

        public MarkdownService(IInteraction dialogService, IFileSystemOperations fileSystemOperations)
        {
            this.dialogService = dialogService;
            this.fileSystemOperations = fileSystemOperations;
        }

        public Task FromFile(string path)
        {
            return dialogService.Message("", fileSystemOperations.ReadAllText(path), "OK".Some(), Path.GetDirectoryName(path).Some());
        }

        public Task Show(string markdown)
        {
            return dialogService.Message("", markdown, "OK".Some(), Optional.Option.None<string>());
        }
    }
}