using Deployer.Execution;
using Grace.DependencyInjection;
using System.Linq;

namespace Deployer.Tests.Utils
{
    internal class TestInstanceBuilder : InstanceBuilder
    {
        public TestInstanceBuilder(ILocatorService container) : base(container)
        {
        }

        public string CreatedInstances { get; set; } = "";

        protected override void OnInstanceCreated(object instance, object[] parameters)
        {
            var toAppend = $"{instance}({string.Join(",", parameters.Select(x => (new Argument(x)).ToString()))})";
            var isFirst = CreatedInstances == string.Empty;
            CreatedInstances = string.Concat(isFirst ? CreatedInstances : "\n" + CreatedInstances, toAppend);
        }
    }
}