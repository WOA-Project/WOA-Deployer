using System;
using Deployer.Core;
using Deployer.Core.Interaction;
using Deployer.Core.Services;
using Deployer.Gui.Services;
using Grace.DependencyInjection;
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
            block.Export<WpfDialogService>().As<IDialogService>().Lifestyle.Singleton();
            block.Export<SettingsService>().As<ISettingsService>().Lifestyle.Singleton();
            block.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
            block.ExportFactory<Uri, IFileSystemOperations, IZafiroFile>((uri, f) => new DesktopZafiroFile(uri, f, null));
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
            block.ExportFactory<string, IContextualizable>((_) => new WpfContextualizable(new Views.Requirements()));
            block.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((s, fso, dl) =>
                new DesktopZafiroFile(new Uri(s), fso, dl));
        }
    }
}
