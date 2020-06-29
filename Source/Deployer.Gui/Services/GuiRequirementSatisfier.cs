using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Disk;
using Zafiro.Core;
using Zafiro.Core.UI;
using Zafiro.Wpf.Services;

namespace Deployer.Gui.Services
{
    public class GuiRequirementSatisfier : IRequirementSatisfier
    {
        private readonly Func<WimPickViewModel> wimPickVmFactory;
        private readonly Func<DiskFillerViewModel> diskFillerVmFactory;
        private readonly Func<SdCardFillerViewModel> sdCardFillerVmFactory;
        private readonly ISimpleInteraction simpleInteraction;

        public GuiRequirementSatisfier(Func<WimPickViewModel> wimPickVmFactory, Func<DiskFillerViewModel> diskFillerVmFactory, Func<SdCardFillerViewModel> sdCardFillerVmFactory, ISimpleInteraction simpleInteraction)
        {
            this.wimPickVmFactory = wimPickVmFactory;
            this.diskFillerVmFactory = diskFillerVmFactory;
            this.sdCardFillerVmFactory = sdCardFillerVmFactory;
            this.simpleInteraction = simpleInteraction;
        }

        public async Task<bool> Satisfy(IDictionary<string, object> unsatisfied)
        {
            var dependenciesModel = new DependenciesModel()
            {
                Children = await GetChildren(unsatisfied.Keys),
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

        private Task<IEnumerable<RequirementFiller>> GetChildren(IEnumerable<string> requirementsToSatisfy)
        {
            var wimPickViewModel = wimPickVmFactory();
            var diskFillerViewModel = diskFillerVmFactory();
            var sdCardFillerViewModel = sdCardFillerVmFactory();

            var viewModels = new RequirementFiller[]
            {
                wimPickViewModel,
                diskFillerViewModel,
                sdCardFillerViewModel,
            };

            var fillers = from req in requirementsToSatisfy
                from v in viewModels
                where v.HandledRequirements.Contains(req, StringComparer.InvariantCultureIgnoreCase)
                select v;

            var vms = fillers.Distinct();

            return Task.FromResult(vms);
        }
    }
}