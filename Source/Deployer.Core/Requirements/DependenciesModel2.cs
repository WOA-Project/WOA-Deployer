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
        private readonly IEnumerable<RequirementSolver> suppliers;

        public DependenciesModel2([NotNull] IEnumerable<RequirementSolver> suppliers)
        {
            this.suppliers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));

            IsValid = Observable.Merge(suppliers.Select(solver => solver.IsValid));
        }

        public IEnumerable<RequirementSolver> Suppliers => suppliers;
        public IObservable<bool> IsValid { get; }
        public bool Continue { get; set; }
    }
}