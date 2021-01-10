using System.Windows;
using System.Windows.Threading;
using Deployer.Core;
using NLog;
using Serilog;
using Serilog.Events;

namespace Deployer.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"Logs\Deployer.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            Log.Information($"Starting Deployer v{AppVersionMixin.VersionString}");
            
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "An unhandled exception has been thrown");
        }
    }
}
