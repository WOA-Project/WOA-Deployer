using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;

namespace Deployer.Core.Requirements
{
    public class DependenciesModel2 : ReactiveObject
    {
        private readonly IEnumerable<IRequirementSolver> solvers;

        public DependenciesModel2([NotNull] IEnumerable<IRequirementSolver> suppliers)
        {
            solvers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));

            IsValid = Observable.Merge(suppliers.Select(solver => solver.IsValid));
        }

        public IEnumerable<IRequirementSolver> Solvers => solvers;
        public IObservable<bool> IsValid { get; }
        public bool Continue { get; set; }
    }
}