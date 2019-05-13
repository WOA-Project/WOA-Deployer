using Serilog.Events;

namespace Deployer.UI.ViewModels
{
    public class RenderedLogEvent
    {
        public string Message { get; set; }
        public LogEventLevel Level { get; set; }
    }
}