using System;
using System.Collections.Generic;

namespace Deployer.Core.Requirements
{
    public abstract class RequirementSolver : IRequirementSolver
    {
        public abstract IObservable<bool> IsValid { get; }
        public abstract IEnumerable<FulfilledRequirement> FulfilledRequirements();
    }
}