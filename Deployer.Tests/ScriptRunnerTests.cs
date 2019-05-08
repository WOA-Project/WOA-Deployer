using System.Linq;
using System.Reflection;
using Deployer.Execution;
using Deployer.Execution.Testing;
using Deployer.Services;
using Deployer.Tasks;
using FluentAssertions;
using Grace.DependencyInjection;
using Xunit;

namespace Deployer.Tests
{
    public class ScriptRunnerTests
    {
        [Fact]
        public void Create()
        {
            var container= new DependencyInjectionContainer();
            container.Configure(x =>
            {
                x.Configure();
                x.ExportFactory(() => "").As<string>();
                x.Export<TestDeploymentContext>().As<IDeploymentContext>();
                x.Export<TestWindowsImageService>().As<IWindowsImageService>();
                x.Export<OperationProgress>().As<IOperationProgress>();
                x.Export<TestPrompt>().As<IPrompt>();
            });

            var parameterTypesOfEveryTask = from ass in new[] {typeof(ShowLicense).Assembly}
                from type in ass.ExportedTypes
                where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeploymentTask))
                let ctor = type.GetTypeInfo().DeclaredConstructors.First()
                from param in ctor.GetParameters()
                select param.ParameterType;

            var attempts = parameterTypesOfEveryTask.Select(x =>
            {
                var tryLocate = container.TryLocate(x, out var l);
                return new {Success = tryLocate, Type = x};
            });

            attempts.ToList().ForEach(x => x.Success.Should().BeTrue($"cannot find {x.Type}"));
        }
    }
}