using System;
using System.Threading.Tasks;
using ReactiveUI;
using Serilog;

namespace Deployer.Gui
{
    public class CommandWrapper<T1, T2> : ReactiveObject
    {
        private readonly object parent;
        private readonly IDialog dialog;
        private readonly ObservableAsPropertyHelper<bool> isExecutingHelper;
        public ReactiveCommand<T1, T2> Command { get; }

        public CommandWrapper(object parent, ReactiveCommand<T1, T2> command, IDialog dialog)
        {
            this.parent = parent;
            this.dialog = dialog;
            Command = command;
            command.ThrownExceptions.Subscribe(async e => await HandleException(e));
            isExecutingHelper = command.IsExecuting.ToProperty(this, x => x.IsExecuting);
        }

        private async Task HandleException(Exception e)
        {
            Log.Error(e, "An error has occurred");
            Log.Information($"Error: {e.Message}");
            await dialog.ShowAlert(parent, Resources.ErrorTitle, $"{e.Message}");   
            Log.Information("");
        }

        public bool IsExecuting => isExecutingHelper.Value;
    }
}