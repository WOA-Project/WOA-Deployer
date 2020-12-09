using System;
using Deployer.Core;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Filesystem;
using Deployer.NetFx;
using Grace.DependencyInjection;
using Zafiro.Core;
using Zafiro.Core.UI;
using Zafiro.Core.UI.Interaction;
using Zafiro.Wpf.Services;

namespace Deployer.Wpf
{
    public class WoaDeployerWpf : WoaDeployerBase
    {
        protected override void ExportSpecificDependencies(IExportRegistrationBlock block)
        {
            block.Export<WpfMarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            block.Export<RequirementsManager>().As<IRequirementsManager>();
            block.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>();
            block.Export<RequirementSupplier>().As<IRequirementSupplier>();
            block.Export<Shell>().As<IShell>();
            block.Export<PopupWindow>().As<IPopup>();
            block.Export<SettingsService>().As<ISettingsService>();
            block.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
            block.Export<WpfDialogService>().As<IDialogService>().Lifestyle.Singleton();
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            block.ExportFactory<string, IContextualizable>(_ => new WpfContextualizable(new Requirements()));
            block.ExportFactory<ResolveSettings, Commands, DeployerFileOpenService, IFileSystem, IRequirementSolver>((settings, commands, fileOpenService, fs) =>
            {
                if (settings.Kind == RequirementKind.WimFile)
                {
                    return new WimPickRequirementSolver(settings.Key, commands, fileOpenService);
                }

                if (settings.Kind == RequirementKind.Disk)
                {
                    return new DiskRequirementSolver(settings.Key, fs);
                }

                throw new ArgumentOutOfRangeException();
            });

        }
    }
}