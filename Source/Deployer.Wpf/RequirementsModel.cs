using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Deployer.Core.Requirements;
using ReactiveUI;

namespace Deployer.Wpf
{
    public class RequirementsModel : ReactiveObject
    {
        private readonly IEnumerable<IRequirementSolver> solvers;

        public RequirementsModel(IEnumerable<IRequirementSolver> suppliers)
        {
            solvers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));

            IsValid = Observable.Merge(suppliers.Select(solver => solver.IsValid));
        }

        public IEnumerable<IRequirementSolver> Solvers => solvers;
        public IObservable<bool> IsValid { get; }
        public bool Continue { get; set; }
    }
}