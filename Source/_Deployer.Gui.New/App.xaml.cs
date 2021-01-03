using System.Windows;
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
                .WriteTo.RollingFile(@"Logs\Log-{Date}.txt")
                .MinimumLevel.Debug()
                .CreateLogger();
        }
    }
}
