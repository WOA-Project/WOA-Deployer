using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;

namespace Deployer.NetFx
{
    public class CompositionRoot
    {
        public static DependencyInjectionContainer CreateContainer()
        {
            var assemblies = Assemblies.AssembliesInAppFolder;
            var configurations = from ass in assemblies
                from exportedType in Try(() => ass.ExportedTypes)
                where typeof(IConfigurationModule).IsAssignableFrom(exportedType)
                where exportedType.IsClass
                let instance = (IConfigurationModule) Activator.CreateInstance(exportedType)
                select instance;

            var container = new DependencyInjectionContainer();

            foreach (var configuration in configurations)
            {
                container.Configure(configuration);
            }

            return container;
        }

        private static IEnumerable<T> Try<T>(Func<IEnumerable<T>> assExportedTypes)
        {
            try
            {
                return assExportedTypes();
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}