using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Deployer.Core.Deployers;
using Deployer.Core.Interaction;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels.Common;
using Grace.DependencyInjection.Attributes;
using Iridio.Runtime;
using ReactiveUI;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI;

namespace Deployer.Gui.ViewModels.Sections
{
    [Metadata("Name", "Advanced")]
    [Metadata("Order", 2)]
    public class AdvancedViewModel : ReactiveObject, ISection
    {
        private readonly BrandNewDeployer deployer;
        private readonly IDialogService dialogService;
        private readonly DeployerFileOpenService fileOpenService;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AdvancedViewModel(BrandNewDeployer deployer, IDialogService dialogService,
            OperationProgressViewModel operationProgress, DeployerFileOpenService fileOpenService)
        {
            this.deployer = deployer;
            this.dialogService = dialogService;
            this.fileOpenService = fileOpenService;
            OperationProgress = operationProgress;

            ConfigureCommands();

            IsBusyObservable = RunScript.IsExecuting;
        }

        public OperationProgressViewModel OperationProgress { get; }

        public ReactiveCommand<Unit, Either<DeployError, Success>> RunScript { get; set; }

        public IObservable<bool> IsBusyObservable { get; }

        private void ConfigureCommands()
        {
            ConfigureRunCommand();
        }

        private void ConfigureRunCommand()
        {
            var filter = new FileTypeFilter("Deployer Script", "*.ds", "*.txt");

            RunScript = ReactiveCommand.CreateFromObservable(() => 
                fileOpenService.Picks("Settings", new []{ filter })
                    .SelectMany(file => deployer.Run(file.Source.OriginalString)));

            RunScript
                .Subscribe(either => either
                    .MapRight(success =>
                        dialogService.Notice("Execution finished", "The script has been executed successfully"))
                    .Handle(errors =>
                        dialogService.Notice("Execution failed", $"The script execution has failed {errors}"))
                    .DisposeWith(disposables));

            dialogService.HandleExceptionsFromCommand(RunScript,
                exception => ("Script execution failed", exception.Message));
        }
    }
}