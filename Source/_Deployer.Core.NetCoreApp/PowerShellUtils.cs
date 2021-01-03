using Microsoft.PowerShell.Cim;

namespace Deployer.Core.NetCoreApp
{
    public static class PowerShellUtils
    {
        private static readonly CimInstanceAdapter Adapter = new CimInstanceAdapter();

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var psAdaptedProperty = Adapter.GetProperty(obj, propertyName);
            if (psAdaptedProperty == null)
            {
                return null;
            }

            return Adapter.GetPropertyValue(psAdaptedProperty);
        }
    }
}