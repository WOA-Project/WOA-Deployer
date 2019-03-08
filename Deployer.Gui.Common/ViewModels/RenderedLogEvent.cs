using Serilog.Events;

namespace Deployer.Gui.Common.ViewModels
{
    public class RenderedLogEvent
    {
        public string Message { get; set; }
        public LogEventLevel Level { get; set; }
    }
}