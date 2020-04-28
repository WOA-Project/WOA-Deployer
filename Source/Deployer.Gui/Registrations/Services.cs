using System;
using Deployer.Core;
using Deployer.Core.FileSystem;
using Deployer.Core.Services;
using Deployer.Gui.Services;
using Grace.DependencyInjection;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;
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
            block.Export<DesktopFilePicker>().As<IFilePicker>().Lifestyle.Singleton();
            block.ExportFactory<Uri, IFileSystemOperations, ZafiroFile>((uri, f) => new DesktopZafiroFile(uri, f, null));
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            var simpleInteraction = new SimpleInteraction();
            simpleInteraction.Register("Requirements", typeof(Requirements));
            block.ExportInstance(simpleInteraction).As<ISimpleInteraction>();
            block.Export<WoaDeployer>().Lifestyle.Singleton();
        }
    }
}