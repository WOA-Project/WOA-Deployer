using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Services;
using Deployer.Gui.Services;
using Grace.DependencyInjection;
using Zafiro.Core;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf.Services;
using Zafiro.Wpf.Services.MarkupWindow;

namespace Deployer.Gui.Registrations
{
    public class GuiServices : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<GuiRequirementSatisfier>().As<IRequirementSatisfier>().Lifestyle.Singleton();
            block.Export<WpfDialogService>().As<IDialogService>().Lifestyle.Singleton();
            block.Export<SettingsService>().As<ISettingsService>().Lifestyle.Singleton();
            block.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            var simpleInteraction = new SimpleInteraction();
            simpleInteraction.Register("Requirements", typeof(Requirements));
            block.ExportInstance(simpleInteraction).As<ISimpleInteraction>();
        }
    }

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
            return dialogService.Interaction("Title", fileSystemOperations.ReadAllText(path), new List<Option>(){  new Option("OK", OptionValue.OK)}, Path.GetDirectoryName(path));
        }

        public Task Show(string markdown)
        {
            return dialogService.Interaction("Title", markdown, new List<Option>(){  new Option("OK", OptionValue.OK)});
        }
    }
}