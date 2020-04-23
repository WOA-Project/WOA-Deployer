using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deployer.Core.Scripting.Core;
using Grace.DependencyInjection;
using SimpleScript;

namespace Deployer.Core.Registrations
{
    public class Functions : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            foreach (var taskType in TaskTypes)
            {
                block.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator)).As<IFunction>();
            }
        }

        public static IEnumerable<Type> TaskTypes
        {
            get
            {
                var taskTypes = from a in new[] { typeof(IDeployerFunction).Assembly }
                                from type in a.ExportedTypes
                                where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeployerFunction))
                                where !type.IsAbstract 
                                select type;
                return taskTypes;
            }
        }
    }
}