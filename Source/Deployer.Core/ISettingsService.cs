using Optional;

namespace Deployer.Core
{
    public interface ISettingsService
    {
        Option<object> this[string key] { get; set; }
    }
}