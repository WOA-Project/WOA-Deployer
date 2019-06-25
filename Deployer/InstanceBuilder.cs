using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;

namespace Deployer
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
            var exceptions = new List<Exception>();
            var providedParameters = new List<object>(parameters);
            var ctors = sourceType.GetTypeInfo().DeclaredConstructors
                        .OrderByDescending(x => x.GetParameters().Count());
            foreach (var ctor in ctors)
            {
                
                var ctorParameters = ctor.GetParameters();
                var ctorParameterTypes = ctorParameters.Select(x => x.ParameterType).ToArray();
                var matchingParameters = new List<object>(providedParameters);
                
                // Handle the case when less parameters are provided than the constructor have.
                while(matchingParameters.Count() < ctorParameterTypes.Count())
                {
                    matchingParameters.Add(null);
                }
                
                // Handle the case when more parameters are provided than the constructor have.
                var injectableParameters = matchingParameters.Take(ctorParameterTypes.Count());

                var zipped = ctorParameterTypes.ZipLongest(injectableParameters, (type, parameterValue) => (Type: type, ParameterValue: parameterValue));

                try
                {
                    var result = new List<object>();
                    foreach(var tuple in zipped)
                    {
                        result.Add(ConvertParam(tuple.Type, tuple.ParameterValue));
                    }
                    return result;
                }
                catch (Exception e)
                {
                    exceptions.Add(new Exception($"Constructor({ConvertParamsToSignature(ctorParameters)}) does not match", e));
                }
            }

            throw new AggregateException("Matching constructor could not be found.", exceptions);
        }

        private object ConvertParam(Type paramType, object value)
        {
            if (paramType == typeof(string))
            {
                return value;
            }

            if (paramType.IsValueType)
            {
                if (value == null)
                {
                    throw new Exception("A value type cannot be null.");
                }
                else if (paramType == value.GetType())
                {
                    return value;
                }
            }

            return container.Locate(paramType);
        }

        private string ConvertParamsToSignature(ParameterInfo[] pInfos)
        {
            var result = new List<string>();
            foreach(var pInfo in pInfos)
            {
                result.Add($"{pInfo.ParameterType.ToString()} {pInfo.Name}");
            }
            return string.Join(", ", result);
        }

        protected virtual void OnInstanceCreated(object instance, object[] parameters)
        {            
        }
    }
}