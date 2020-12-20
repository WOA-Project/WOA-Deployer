using System;
using Deployer.Core;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Filesystem;
using Deployer.Net4x;
using Grace.DependencyInjection;
using Serilog;
using Zafiro.UI;
using Zafiro.UI.Wpf;

namespace Deployer.Wpf
{
    public class WoaDeployerWpf : WoaDeployer
    {
        protected override void ExportSpecificDependencies(IExportRegistrationBlock block)
        {
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            block.Export<RequirementsManager>().As<IRequirementsManager>();
            block.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>();
            block.Export<RequirementSupplier>().As<IRequirementSupplier>();
            block.Export<PopupWindow>().As<IView>();
            block.Export<Popup>().As<IPopup>();
            block.Export<SettingsService>().As<ISettingsService>();
            block.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
            block.Export<Interaction>().As<IInteraction>().Lifestyle.Singleton();
            block.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
            block.ExportFactory<string, IHaveDataContext>(_ => new HaveDataContextAdapter(new Requirements()));
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