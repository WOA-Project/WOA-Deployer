using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional.Collections;
using Optional.Unsafe;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI.Interaction;
using Option = Zafiro.Core.UI.Interaction.Option;

namespace Deployer.Core.Requirements
{
    public class RequirementSupplier 
    {
        private readonly IDictionary<RequirementKind, RequirementSolver> map;
        private readonly IShell shell;
        private readonly Func<IContextualizable> content;

        public RequirementSupplier(IDictionary<RequirementKind, RequirementSolver> map, IShell shell, Func<IContextualizable> content)
        {
            this.map = map;
            this.shell = shell;
            this.content = content;
        }

        public async Task<Either<Error, IEnumerable<FulfilledRequirement>>> Satisfy(IEnumerable<MissingRequirement> requirements)
        {
            var individualSuppliers = requirements.Select(SupplyFor);
            var vm = new DependenciesModel2(individualSuppliers);

            var contextualizable = content();
            await shell.Popup(contextualizable, vm,
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
            return vm.Suppliers.SelectMany(Extract);
        }

        private IEnumerable<FulfilledRequirement> Extract(IRequirementSolver vm)
        {
            return vm.FulfilledRequirements();
        }

        private RequirementSolver SupplyFor(MissingRequirement missingRequirement)
        {
            return DictionaryExtensions.GetValueOrNone(map, missingRequirement.Kind).ValueOrFailure();
        }
    }
}