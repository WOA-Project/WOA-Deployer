using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Option;
using Deployer.Core;
using Deployer.Core.Deployers;
using Deployer.Core.Interaction;
using Deployer.Core.Services;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels.Common;
using Deployer.Gui.ViewModels.Sections;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Common;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;
using Zafiro.Wpf.Services;
using Requirements = Deployer.Core.Registrations.Requirements;

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
            block.Export<PopupWindow>().As<IPopup>();
            block.ExportFactory<IContextualizable>(() => new WpfContextualizable(new Views.Requirements()));

        }
    }
}
