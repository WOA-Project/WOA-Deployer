using System;
using Deployer.Core.FileSystem;
using Deployer.Core.Requirements;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels.Common;
using Deployer.Gui.ViewModels.RequirementSolvers;
using Grace.DependencyInjection;

namespace Deployer.Gui.Registrations
{
    public class RequirementSolvers : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock registrationBlock)
        {
            registrationBlock.Export<WimPickRequirementSolver>().Lifestyle.Singleton();
            registrationBlock.ExportFactory<ResolveSettings, Commands, DeployerFileOpenService, IFileSystem, IRequirementSolver>((settings, commands, fileOpenService, fs) =>
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