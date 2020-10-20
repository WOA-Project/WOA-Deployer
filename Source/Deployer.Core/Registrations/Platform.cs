using System;
using Grace.DependencyInjection;
using Serilog.Events;
using Zafiro.Core.UI.Interaction;

namespace Deployer.Core.Registrations
{
    public class Platform : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.ExportFactory(() => LogEventSource.Current).As<IObservable<LogEvent>>().Lifestyle.Singleton();
            block.Export<LogCollector>().As<ILogCollector>().Lifestyle.Singleton();
            block.Export<ShellOpen>().As<IShellOpen>().Lifestyle.Singleton();
            block.Export<Shell>().As<IShell>().Lifestyle.Singleton();
        }
    }
}