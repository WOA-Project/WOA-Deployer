using System.Collections.Generic;
using Deployer.Core.Deployers.Errors.Compiler;

namespace Deployer.Core.Deployers.Errors.Deployer
{
    public class RequirementsFailed : DeployerError
    {
        public UnableToSatisfyRequirements RequirementsError { get; }

        public RequirementsFailed(UnableToSatisfyRequirements requirementsError)
        {
            RequirementsError = requirementsError;
        }

        public override IEnumerable<string> Items => RequirementsError.Items;
    }
}