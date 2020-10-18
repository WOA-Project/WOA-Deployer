using System;
using System.Collections.Generic;

namespace Deployer.Core.Requirements
{
    public interface IRequirementSolver
    {
        IObservable<bool> IsValid { get; }
        IEnumerable<FulfilledRequirement> FulfilledRequirements();
    }
}