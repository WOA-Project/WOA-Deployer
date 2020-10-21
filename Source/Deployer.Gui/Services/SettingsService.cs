using Deployer.Core;
using Optional;

namespace Deployer.Gui.Services
{
    public class SettingsService : ISettingsService
    {
        public Option<object> this[string key]
        {
            get => GetSetting(key);
            set => SetSetting(key, value);
        }

        private Option<object> GetSetting(string key)
        {
            return Option.None<object>();
        }

        private void SetSetting(string key, Option<object> value)
        {
        }
    }
}