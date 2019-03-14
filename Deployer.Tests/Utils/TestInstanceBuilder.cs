using Grace.DependencyInjection;

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
            var toAppend = $"{instance}({string.Join(",", parameters)})";
            var isFirst = CreatedInstances == string.Empty;
            CreatedInstances = string.Concat(isFirst ? CreatedInstances : "\n" + CreatedInstances, toAppend);
        }
    }
}