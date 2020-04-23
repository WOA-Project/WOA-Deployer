using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Disk;
using Zafiro.Core;
using Zafiro.Wpf.Services;

namespace Deployer.Gui.Services
{
    public class GuiRequirementSatisfier : IRequirementSatisfier
    {
        private readonly Func<WimPickViewModel> wimPickVmFactory;
        private readonly Func<DiskFillerViewModel> diskFillerVmFactory;
        private readonly ISimpleInteraction simpleInteraction;

        public GuiRequirementSatisfier(Func<WimPickViewModel> wimPickVmFactory, Func<DiskFillerViewModel> diskFillerVmFactory, ISimpleInteraction simpleInteraction)
        {
            this.wimPickVmFactory = wimPickVmFactory;
            this.diskFillerVmFactory = diskFillerVmFactory;
            this.simpleInteraction = simpleInteraction;
        }

        public async Task<bool> Satisfy(IDictionary<string, object> unsatisfied)
        {
            var dependenciesModel = new DependenciesModel()
            {
                Children = await GetChildren(unsatisfied.Keys, unsatisfied),
            };
            var cont = false;

            var reqs = new ViewModels.Requirements(unsatisfied);

            foreach (var dependenciesModelChild in dependenciesModel.Children)
            {
                dependenciesModelChild.Requirements = reqs;
            }

            var dialogButtons = new List<DialogButton>()
            {
                new DialogButton
                {
                    Handler = c =>
                    {
                        cont = true;
                        c.Close();
                    },
                    CanExecute = reqs.Satisfied,
                    Text = "OK",
                },
                new DialogButton()
                {
                    Handler = c => { c.Close(); },
                    CanExecute = Observable.Return(true),
                    Text = "Cancel",
                }
            };

            await simpleInteraction.Interact("Requirements", "Requirements", dependenciesModel, dialogButtons);
            return cont;
        }

        private Task<IEnumerable<RequirementFiller>> GetChildren(IEnumerable<string> requirementsToSatisfy,
            IDictionary<string, object> unsatisfied)
        {
            var wimPickViewModel = wimPickVmFactory();
            var diskFillerViewModel = diskFillerVmFactory();

            var viewModels = new RequirementFiller[]
            {
                wimPickViewModel,
                diskFillerViewModel,
            };

            var fillers = from req in requirementsToSatisfy
                from v in viewModels
                where v.HandledRequirements.Contains(req)
                select v;

            var vms = fillers.Distinct();

            return Task.FromResult(vms);
        }
    }
}