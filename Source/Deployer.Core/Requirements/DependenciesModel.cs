using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace Deployer.Core.Requirements
{
    public class DependenciesModel : ReactiveObject
    {
        private readonly IEnumerable<IRequirementSolver> solvers;

        public DependenciesModel(IEnumerable<IRequirementSolver> requirementSolvers)
        {
            solvers = requirementSolvers ?? throw new ArgumentNullException(nameof(requirementSolvers));

            IsValid = requirementSolvers.Select(solver => solver.IsValid).CombineLatest(list => list.All(value => value == true));
        }

        public IEnumerable<IRequirementSolver> Solvers => solvers;
        public IObservable<bool> IsValid { get; }
        public bool Continue { get; set; }
    }
}