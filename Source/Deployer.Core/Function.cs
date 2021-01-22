using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Common.Utils;
using Iridio.Parsing.Model;
using Serilog;

namespace Deployer.Core
{
    public class Function : IFunction, IFunctionDeclaration
    {
        private readonly Lazy<object> instance;
        private readonly MethodInfo method;

        public Function(Type type, Func<Type, object> locator)
        {
            instance = new Lazy<object>(() => locator(type));
            Name = type.Name;
            method = type.GetMethod("Execute");
            if (method == null)
            {
                throw new InvalidOperationException($@"Cannot find the Execute method on type {type}");
            }
        }

        public async Task<object> Invoke(object[] parameters)
        {
            Log.Information("Executing {Name} with parameters {Parameters}", Name, Arguments.Zip(parameters, (argument, value) => new { Argument = argument, Value = value }));
            var transformed = parameters.Zip(method.GetParameters(), Transform);
            var result = await method.InvokeTask(instance.Value, transformed.ToArray());
            return result;
        }

        public string Name { get; }

        public IEnumerable<Argument> Arguments =>
            method.GetParameters().Select(x => new Argument(x.Name, x.ParameterType));

        public Type ReturnType => method.ReturnType;

        private object Transform(object o, ParameterInfo info)
        {
            if (info.ParameterType == typeof(int))
            {
                if (o is int n)
                {
                    return n;
                }

                return int.Parse((string) o);
            }

            if (info.ParameterType == typeof(double))
            {
                if (o is double n)
                {
                    return n;
                }

                return double.Parse((string)o);
            }

            return o;
        }

        public override string ToString()
        {
            var parameterList = method.GetParameters().Select(info => $"{info.ParameterType.Name} {info.Name}");
            var firm = $"({string.Join(",", parameterList)})";
            var returnType = method.ReturnType == typeof(Task)
                ? ""
                : method.ReturnType.GenericTypeArguments.FirstOrDefault()?.Name ?? "Unknown";
            return $"{returnType}\t{Name}{firm}";
        }
    }
}