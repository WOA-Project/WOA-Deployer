using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Deployer.Core.Requirements;
using Deployer.Filesystem;
using ReactiveUI;
using IFileSystem = Deployer.Filesystem.IFileSystem;
using Unit = System.Reactive.Unit;

namespace Deployer.Wpf
{
    public class DiskRequirementSolver : ReactiveObject, IRequirementSolver
    {
        private readonly string key;
        private readonly ObservableAsPropertyHelper<bool> isBusy;
        private DiskViewModel selectedDisk;
        private readonly IObservable<bool> isValid;
        private ObservableAsPropertyHelper<IEnumerable<DiskViewModel>> disks;

        public DiskRequirementSolver(string key, IFileSystem fileSystem)
        {
            this.key = key;
            RefreshDisks = ReactiveCommand.CreateFromTask(fileSystem.GetDisks);
            disks = RefreshDisks
                .Select(x => Enumerable.Select(x, disk => new DiskViewModel(disk)))
                .ToProperty(this, x => x.Disks);

            isBusy = RefreshDisks.IsExecuting.ToProperty(this, x => x.IsBusy);
            isValid = this.WhenAnyValue(x => x.SelectedDisk).Select(model => model != null);
        }

        public bool IsBusy => isBusy.Value;

        public IEnumerable<DiskViewModel> Disks => disks.Value;

        public ReactiveCommand<Unit, IList<IDisk>> RefreshDisks { get; }

        public DiskViewModel SelectedDisk
        {
            get => selectedDisk;
            set => this.RaiseAndSetIfChanged(ref selectedDisk, value);
        }
        
        public virtual IObservable<bool> IsValid => isValid;

        public virtual IEnumerable<FulfilledRequirement> FulfilledRequirements()
        {
            return new[]
            {
                new FulfilledRequirement(key, SelectedDisk.Number - 1),
            };
        }
    }
}