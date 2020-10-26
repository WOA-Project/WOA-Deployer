using System.Collections.ObjectModel;

namespace Deployer.Core.Requirements
{
    public class RequirementResponse : Collection<FulfilledRequirement>
    {
        public RequirementResponse(FulfilledRequirement[] fulfilledRequirements) : base(fulfilledRequirements)
        {
        }
    }
}