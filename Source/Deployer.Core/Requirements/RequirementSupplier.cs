using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI.Interaction;
using Option = Zafiro.Core.UI.Interaction.Option;

namespace Deployer.Core.Requirements
{
    public class RequirementSupplier : IRequirementSupplier
    {
        private readonly Func<ResolveSettings, IRequirementSolver> solverFactory;
        private readonly IShell shell;
        private readonly Func<IContextualizable> contentFactory;

        public RequirementSupplier(Func<ResolveSettings, IRequirementSolver> solverFactory, IShell shell, Func<IContextualizable> contentFactory)
        {
            this.solverFactory = solverFactory;
            this.shell = shell;
            this.contentFactory = contentFactory;
        }

        public async Task<Either<Error, IEnumerable<FulfilledRequirement>>> Satisfy(IEnumerable<MissingRequirement> requirements)
        {
            var individualSuppliers = requirements.Select(Supplier).ToList();
            var vm = new DependenciesModel2(individualSuppliers);

            await shell.Popup(contentFactory(), vm,
                c =>
                {
                    c.Popup.Title = "Please, specify the following information";
                    c.AddOption(new Option("OK", ReactiveCommand.Create(() =>
                    {
                        c.Model.Continue = true;
                        c.Popup.Close();
                    }, c.Model.IsValid)));
                });

            if (vm.Continue)
            {
                return Either.Success<Error, IEnumerable<FulfilledRequirement>>(Extract(vm));
            }

            return Either.Error<Error, IEnumerable<FulfilledRequirement>>(new Error());
        }

        private IEnumerable<FulfilledRequirement> Extract(DependenciesModel2 vm)
        {
            return vm.Solvers.SelectMany(Extract);
        }

        private IEnumerable<FulfilledRequirement> Extract(IRequirementSolver vm)
        {
            return vm.FulfilledRequirements();
        }

        private IRequirementSolver Supplier(MissingRequirement missingRequirement)
        {
            return solverFactory(new ResolveSettings(missingRequirement.Key, missingRequirement.Kind));
        }
    }
}