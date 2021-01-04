using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Deployer.Core.Interaction;
using Deployer.Gui.Properties;
using Deployer.Wpf;
using Grace.DependencyInjection;
using Optional;
using ReactiveUI;
using Zafiro.UI;

namespace Deployer.Gui.ViewModels.Sections
{
    public class MainViewModel : MainViewModelBase
    {
        public OperationStatusViewModel OperationStatus { get; }

        public MainViewModel(IList<Meta<ISection>> sections, IList<IBusy> busies, IInteraction interaction, OperationStatusViewModel operationStatus) : base(sections, busies)
        {
            OperationStatus = operationStatus;
            ShowWarning = ReactiveCommand.CreateFromObservable(() => 
                Observable.FromAsync(() =>  interaction.Message(Resources.TermsOfUseTitle, Resources.WarningNotice, "OK".Some(),
                        Optional.Option.None<string>())));
        }

        public ReactiveCommand<Unit, Unit> ShowWarning { get; set; }

        protected override string DonationLink => "https://github.com/WoA-project/WOA-Deployer/blob/master/Docs/Donations.md";
        protected override string HelpLink => "https://github.com/WOA-Project/WOA-Deployer-Lumia#need-help";
        public override string Title => AppProperties.AppTitle;
    }
}