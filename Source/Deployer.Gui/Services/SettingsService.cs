using System.Configuration;
using Deployer.Core;
using Optional;
using Optional.Unsafe;

namespace Deployer.Gui.Services
{
    public class SettingsService : ISettingsService
    {
        public Option<object> this[string key]
        {
            get
            {
                var defaultValue = Properties.Settings.Default.Properties[key]?.DefaultValue;
                var someNotNull = defaultValue.SomeNotNull();
                return someNotNull;
            }
            set
            {
                var prop =
                    Properties.Settings.Default.Properties[key].SomeNotNull()
                        .Match(s => s, () =>
                        {
                            var newProp = new SettingsProperty(key) { PropertyType = value.GetType() };
                            Properties.Settings.Default.Properties.Add(newProp);
                            return newProp;
                        });

                prop.DefaultValue = value.ValueOrDefault();
                Properties.Settings.Default.Save();
            }
        }
    }
}