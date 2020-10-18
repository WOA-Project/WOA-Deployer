using System.Collections.Generic;
using System.Reactive;
using Deployer.Core.Interaction;
using Deployer.Gui.New;
using Deployer.Gui.ViewModels.Common;
using Grace.DependencyInjection;
using ReactiveUI;
using Zafiro.Core.UI;

namespace Deployer.Gui.ViewModels.Sections
{
    public class MainViewModel : MainViewModelBase
    {
        public OperationProgressViewModel OperationProgress { get; }

        public MainViewModel(IList<Meta<ISection>> sections, IList<IBusy> busies, IDialogService contextDialog, OperationProgressViewModel operationProgress) : base(sections, busies)
        {
            OperationProgress = operationProgress;
            ShowWarningCommand = ReactiveCommand.CreateFromTask(() => contextDialog.Notice(Resources.TermsOfUseTitle, Resources.WarningNotice));
        }

        public ReactiveCommand<Unit, Unit> ShowWarningCommand { get; set; }

        protected override string DonationLink => "https://github.com/WoA-project/WOA-Deployer/blob/master/Docs/Donations.md";
        protected override string HelpLink => "https://github.com/WOA-Project/WOA-Deployer-Lumia#need-help";
        public override string Title => AppProperties.AppTitle;
    }
}