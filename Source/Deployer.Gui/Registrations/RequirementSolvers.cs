using System;
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
            registrationBlock.ExportFactory<ResolveSettings, Commands, DeployerFileOpenService, IRequirementSolver>((settings, commands, fileOpenService) =>
            {
                if (settings.Kind == RequirementKind.WimFile)
                {
                    return new WimPickRequirementSolver(settings.Key, commands, fileOpenService);
                }

                throw new ArgumentOutOfRangeException();
            });
        }
    }
}