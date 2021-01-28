using System;
using System.Collections.Generic;
using System.Reflection;
using Deployer.Core;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Net4x;
using Grace.DependencyInjection;
using Zafiro.UI;
using Zafiro.UI.Wpf;

namespace Deployer.Wpf
{
    public class WoaDeployerWpf : WoaDeployer
    {
        public WoaDeployerWpf(IEnumerable<Assembly> assembliesToScan) : base(assembliesToScan)
        {
        }

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
            block.ExportFactory<ResolveSettings, IExportLocatorScope, IRequirementSolver>((settings, e) =>
            {
                if (settings.Definition == RequirementDefinition.WimFile)
                    return e.Locate<WimPickRequirementSolver>(new {settings.Key});

                if (settings.Definition == RequirementDefinition.Disk) return e.Locate<DiskRequirementSolver>(new {settings.Key});

                if (settings.Definition is DoubleNumberRequirementDefinition def) 
                    return e.Locate<DoubleNumberRequirementSolver>(new { settings.Key, def.Min, def.DefaultValue, def.Max, settings.Definition, settings.Description });

                throw new ArgumentOutOfRangeException();
            });
        }
    }
}