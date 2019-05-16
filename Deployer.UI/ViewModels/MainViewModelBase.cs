using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Grace.DependencyInjection;
using ReactiveUI;

namespace Deployer.UI.ViewModels
{
    public abstract class MainViewModelBase : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> isBusyHelper;
        private Meta<ISection> selectedItem;
        private readonly ObservableAsPropertyHelper<bool> isBigProgressVisible;
        protected abstract string DonationLink { get; }
        protected abstract string HelpLink { get; }

        protected MainViewModelBase(IList<Meta<ISection>> sections, IList<IBusy> busies)
        {
            if (!busies.Any())
            {
                throw new ArgumentException("We should get at least one Busy item!");
            }

            var isBusyObs = busies.Select(x => x.IsBusyObservable).Merge();

            DonateCommand = ReactiveCommand.Create(() => { Process.Start(DonationLink); });
            HelpCommand = ReactiveCommand.Create(() => { Process.Start(HelpLink); });
            isBusyHelper = isBusyObs.ToProperty(this, model => model.IsBusy);
            Sections = sections.OrderBy(meta => (int)meta.Metadata["Order"]).ToList();
            isBigProgressVisible = this.WhenAnyValue(x => x.SelectedItem)
                .CombineLatest(isBusyObs, (section, busy) => section != null && (int)section.Metadata["Order"] == 0 && busy)
                .ToProperty(this, x => x.IsBigProgressVisible);
        }

        public IList<Meta<ISection>> Sections { get; set; }

        public bool IsBusy => isBusyHelper.Value;

        public ReactiveCommand<Unit, Unit> DonateCommand { get; }

        public abstract string Title { get; }

        public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }

        public bool IsBigProgressVisible => isBigProgressVisible.Value;

        public Meta<ISection> SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }
    }
}