using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Deployer.Core.FileSystem;
using ReactiveUI;

namespace Deployer.Gui.ViewModels.Common.Disk
{
    public abstract class DiskFillerViewModelBase : RequirementFiller
    {
        private readonly ObservableAsPropertyHelper<IEnumerable<DiskViewModel>> disks;
        private DiskViewModel selectedDisk;
        private readonly ObservableAsPropertyHelper<bool> isBusy;

        public DiskFillerViewModelBase(IFileSystem fileSystem)
        {
            RefreshDisks = ReactiveCommand.CreateFromTask(fileSystem.GetDisks);
            disks = RefreshDisks
                .Select(x => Enumerable.Select<IDisk, DiskViewModel>(x, disk => new DiskViewModel(disk)))
                .ToProperty(this, x => x.Disks);

            isBusy = RefreshDisks.IsExecuting.ToProperty(this, x => x.IsBusy);
            this.WhenAnyValue(x => x.SelectedDisk).Subscribe(onNext: x =>
            {
                if (x != null)
                {
                    Requirements[AssociatedRequirement] = x.Number - 1;
                }
            });
        }

        public abstract string AssociatedRequirement { get; }

        public bool IsBusy => isBusy.Value;

        public IEnumerable<DiskViewModel> Disks => disks.Value;

        public DiskViewModel SelectedDisk
        {
            get => selectedDisk;
            set => this.RaiseAndSetIfChanged(ref selectedDisk, value);
        }

        public ReactiveCommand<Unit, IList<IDisk>> RefreshDisks { get; }

        public override IEnumerable<string> HandledRequirements => new[] { AssociatedRequirement };
    }
}