using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Optional.Collections;
using Optional.Unsafe;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI;

namespace Deployer.Tests
{
    public class RequirementSupplier 
    {
        private readonly IDictionary<RequirementKind, RequirementSolver> map;
        private readonly ISimpleInteraction interaction;

        public RequirementSupplier(IDictionary<RequirementKind, RequirementSolver> map, ISimpleInteraction interaction)
        {
            this.map = map;
            this.interaction = interaction;
        }

        public async Task<Either<Error, IEnumerable<FulfilledRequirement>>> Satisfy(IEnumerable<MissingRequirement> requirements)
        {
            var individualSuppliers = requirements.Select(SupplyFor);
            var vm = new DependenciesModel2(individualSuppliers);


            ICollection<DialogButton> buttons = null;
            await interaction.Interact("Requirements", "Please, specify the following information", vm, buttons);
            return Either.Success<Error, IEnumerable<FulfilledRequirement>>(Extract(vm));
        }

        private IEnumerable<FulfilledRequirement> Extract(DependenciesModel2 vm)
        {
            return vm.Children.SelectMany(x => Extract(x));
        }

        private IEnumerable<FulfilledRequirement> Extract(RequirementSolver vm)
        {
            return vm switch
            {
                WimPickRequirementSolver requirementSolverImpl => new List<FulfilledRequirement>(),
                _ => throw new ArgumentOutOfRangeException(nameof(vm))
            };
        }

        private RequirementSolver SupplyFor(MissingRequirement missingRequirement)
        {
            return DictionaryExtensions.GetValueOrNone(map, missingRequirement.Kind).ValueOrFailure();
        }

        public List<DialogButton> GetButtons()
        {
            var dialogButtons = new List<DialogButton>()
            {
                new DialogButton
                {
                    Handler = c =>
                    {
                        //cont = true;
                        c.Close();
                    },
                    //CanExecute = reqs.Satisfied,
                    Text = "OK",
                },
                new DialogButton()
                {
                    Handler = c => { c.Close(); },
                    CanExecute = Observable.Return(true),
                    Text = "Cancel",
                }
            };

            return dialogButtons;
        }
    }

    public class Error
    {
    }

    public class DependenciesModel2
    {
        private readonly IEnumerable<RequirementSolver> suppliers;

        public DependenciesModel2(IEnumerable<RequirementSolver> suppliers)
        {
            this.suppliers = suppliers;
        }

        public IEnumerable<RequirementSolver> Children { get; set; }
    }
}