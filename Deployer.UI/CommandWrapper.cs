using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Tasks;
using Deployer.UI.Properties;
using ReactiveUI;
using Serilog;

namespace Deployer.UI
{
    public class CommandWrapper<T1, T2> : ReactiveObject
    {
        private readonly object parent;
        private readonly IContextDialog dialog;
        private readonly IDeploymentContext context;
        private readonly ObservableAsPropertyHelper<bool> isExecutingHelper;
        public ReactiveCommand<T1, T2> Command { get; }

        public CommandWrapper(object parent, ReactiveCommand<T1, T2> command, IContextDialog dialog, IDeploymentContext context)
        {
            this.parent = parent;
            this.dialog = dialog;
            this.context = context;
            Command = command;
            command.ThrownExceptions.Subscribe(async e => await HandleException(e));
            isExecutingHelper = command.IsExecuting.ToProperty(this, x => x.IsExecuting);
        }

        private async Task HandleException(Exception e)
        {
            if (e is TaskCanceledException)
            {
                Log.Error(e, "An error has occurred");
                await dialog.ShowAlert(parent, "Operation cancelled", "The operation has been cancelled");
                context.Cancelled.OnNext(Unit.Default);
            }
            else
            {
                Log.Error(e, "An error has occurred");
                Log.Information($"Error: {e.Message}");
                await dialog.ShowAlert(parent, Resources.ErrorTitle, $"{e.Message}");   
                Log.Information("");
            }
        }

        public bool IsExecuting => isExecutingHelper.Value;
    }
}