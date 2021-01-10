using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Core.Requirements
{
    public interface IRequirementSolver
    {
        IObservable<bool> IsValid { get; }
        Task<RequirementResponse> FulfilledRequirements();
    }
}