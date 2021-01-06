using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Deployer.Core.Requirements
{
    public class RequirementResponse : Collection<FulfilledRequirement>
    {
        public RequirementResponse(IList<FulfilledRequirement> fulfilledRequirements) : base(fulfilledRequirements)
        {
        }
    }
}