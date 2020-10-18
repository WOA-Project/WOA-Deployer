using System;
using System.Linq;
using BuildingBlocks.Option;
using Deployer.Core;
using Deployer.Core.Interaction;
using Deployer.Core.NetCoreApp;
using Deployer.Core.Services;
using Deployer.Gui.Services;
using Grace.DependencyInjection;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;
using Zafiro.Wpf.Services;

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
            block.Export<ScriptDeployer>().As<Core.Deployer>().Lifestyle.Singleton();
            block.Export<DeviceDeployer>().As<Core.Deployer>().Lifestyle.Singleton();
            block.ExportAssemblies(new[] { typeof(ViewModels.Sections.MainViewModel).Assembly })
                .Where(y =>
                {
                    var isSection = typeof(ISection).IsAssignableFrom(y);
                    return isSection;
                })
                .ByInterface<ISection>()
                .ByInterface<IBusy>()
                .ByType()
                .ExportAttributedTypes()
                .Lifestyle.Singleton();
        }
    }
}