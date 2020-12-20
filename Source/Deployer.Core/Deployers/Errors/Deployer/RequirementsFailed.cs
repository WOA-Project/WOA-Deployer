using System.Collections.Generic;
using System.Linq;
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

        public override string ToString()
        {
            return string.Join(";", Items.Select(s => s));
        }
    }
}