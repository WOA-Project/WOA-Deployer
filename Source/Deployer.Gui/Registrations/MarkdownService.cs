using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Services;
using Zafiro.Core;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;

namespace Deployer.Gui.Registrations
{
    public class MarkdownService : IMarkdownService
    {
        private readonly IDialogService dialogService;
        private readonly IFileSystemOperations fileSystemOperations;

        public MarkdownService(IDialogService dialogService, IFileSystemOperations fileSystemOperations)
        {
            this.dialogService = dialogService;
            this.fileSystemOperations = fileSystemOperations;
        }

        public Task FromFile(string path)
        {
            return dialogService.Interaction("", fileSystemOperations.ReadAllText(path), new List<Option>(){  new Option("OK", OptionValue.OK)}, Path.GetDirectoryName(path));
        }

        public Task Show(string markdown)
        {
            return dialogService.Interaction("", markdown, new List<Option>(){  new Option("OK", OptionValue.OK)});
        }
    }
}