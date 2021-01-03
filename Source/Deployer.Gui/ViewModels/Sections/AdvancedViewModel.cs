using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Deployer.Core;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Interaction;
using Deployer.Wpf;
using Grace.DependencyInjection.Attributes;
using Iridio.Runtime.ReturnValues;
using Optional;
using ReactiveUI;
using Zafiro.Core.Patterns.Either;
using Zafiro.UI;
using DeployerFileOpenService = Deployer.Gui.Services.DeployerFileOpenService;

namespace Deployer.Gui.ViewModels.Sections
{
    [Metadata("Name", "Advanced")]
    [Metadata("Order", 2)]
    public class AdvancedViewModel : ReactiveObject, ISection
    {
        private readonly IWoaDeployer deployer;
        private readonly IInteraction interaction;
        private readonly DeployerFileOpenService fileOpenService;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AdvancedViewModel(IWoaDeployer deployer, IInteraction interaction,
            OperationProgressViewModel operationProgress, DeployerFileOpenService fileOpenService)
        {
            this.deployer = deployer;
            this.interaction = interaction;
            this.fileOpenService = fileOpenService;
            OperationProgress = operationProgress;

            ConfigureCommands();

            IsBusyObservable = RunScript.IsExecuting;
        }

        public OperationProgressViewModel OperationProgress { get; }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> RunScript { get; set; }

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
                        interaction.Message("Execution finished", "The script has been executed successfully", "OK".Some(), Optional.Option.None<string>()))
                    .Handle(errors =>
                        interaction.Message("Execution failed", $"The script execution has failed: {errors}", "OK".Some(), Optional.Option.None<string>())))
                    .DisposeWith(disposables);
        }
    }
}