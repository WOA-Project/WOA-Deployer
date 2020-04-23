using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using MoreLinq.Extensions;

namespace Deployer.Core
{
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly ILocatorService container;

        public InstanceBuilder(ILocatorService container)
        {
            this.container = container;
        }

        public object Create(Type type, params object[] parameters)
        {
            var injectableParameters = GetParametersToInject(type, parameters);
            var finalParameters = injectableParameters.ToArray();
            var instance = Activator.CreateInstance(type, finalParameters);
            OnInstanceCreated(instance, finalParameters);
            return instance;
        }

        private IEnumerable<object> GetParametersToInject(Type sourceType, IEnumerable<object> parameters)
        {
            var ctor = sourceType.GetTypeInfo().DeclaredConstructors.First();
            var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType);
            var injectableParameters = parameters.Take(ctor.GetParameters().Length);
            var zipped = parameterTypes.ZipLongest(injectableParameters, (type, parameterValue) => (Type: type, ParameterValue: parameterValue));

            return from tuple in zipped
                let final = ConvertParam(tuple.Type, tuple.ParameterValue)
                select final;
        }

        private object ConvertParam(Type paramType, object value)
        {
            if (paramType == typeof(string))
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Invalid parameters provided");
                }

                return value;
            }

            return container.Locate(paramType);
        }

        protected virtual void OnInstanceCreated(object instance, object[] parameters)
        {            
        }
    }
}