using Deployer.Core.Requirements;
using Grace.DependencyInjection;

namespace Deployer.Core.Registrations
{
    public class Requirements : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<RequirementSupplier>().As<IRequirementSupplier>();
            block.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>();
        }
    }
}