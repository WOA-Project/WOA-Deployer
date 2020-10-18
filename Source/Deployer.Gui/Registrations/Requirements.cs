using Deployer.Core.Requirements;
using Grace.DependencyInjection;

namespace Deployer.Gui.Registrations
{
    public class Requirements : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<RequirementSupplier>().As<IRequirementSupplier>();
        }
    }
}