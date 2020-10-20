using System;
using System.Collections.Generic;
using System.Reactive;
using Deployer.Core.Requirements;
using Deployer.Gui.ViewModels.Common;
using ReactiveUI;

namespace Deployer.Gui.ViewModels.RequirementSolvers
{
    public class WimPickRequirementSolver : ReactiveObject, IRequirementSolver
    {
        private readonly string key;
        private readonly ObservableAsPropertyHelper<bool> hasWimHelper;
        private readonly IObservable<bool> isValid;
        private readonly ObservableAsPropertyHelper<WimMetadataViewModel> pickWimFileObs;

        public WimPickRequirementSolver(string key, Commands commands)
        {
            this.key = key;
            OpenGetWoaCommand = commands.ShellOpen;
            PickWimFileCommand = ReactiveCommand.CreateFromObservable(() => commands.GetPickWimFileObs(key));
            pickWimFileObs = PickWimFileCommand.ToProperty(this, x => x.WimMetadata);
            isValid = this.WhenAnyValue(model => model.WimMetadata, (WimMetadataViewModel x) => x != null);
            hasWimHelper = isValid
                .ToProperty(this, x => x.HasWim);
        }

        public ReactiveCommand<Unit, WimMetadataViewModel> PickWimFileCommand { get; set; }
        public IReactiveCommand OpenGetWoaCommand { get; }
        public bool HasWim => hasWimHelper.Value;
        public WimMetadataViewModel WimMetadata => pickWimFileObs.Value;
        public virtual IObservable<bool> IsValid => isValid;
        public RequirementKind HandledRequirement => RequirementKind.WimFile;

        public virtual IEnumerable<FulfilledRequirement> FulfilledRequirements()
        {
            return new[]
            {
                new FulfilledRequirement(key + "Path", WimMetadata.Path),
                new FulfilledRequirement(key + "Index", WimMetadata.SelectedDiskImage.Index),
            };
        }
    }
}