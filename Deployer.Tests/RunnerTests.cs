using System.Linq;
using System.Threading.Tasks;
using Deployer.Execution;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class RunnerTests
    {
        [Fact]
        public async Task Simple()
        {
            var testtask = "TestTask";
            
            var testInstanceBuilder = await TestInstanceBuilder(testtask);

            testInstanceBuilder.CreatedInstances.Single().Should().BeOfType<TestTask>();
        }

        [Fact]
        public async Task Parameterized()
        {
            var testTask = "ParameterizedTask \"something\"";

            var testInstanceBuilder = await TestInstanceBuilder(testTask);

            testInstanceBuilder.CreatedInstances.Single().Should().BeOfType<ParameterizedTask>();
        }

        private static async Task<TestInstanceBuilder> TestInstanceBuilder(string testTask)
        {
            var parser = new ScriptParser(Tokenizer.Create());
            var script = parser.Parse(testTask);
            var testInstanceBuilder = new TestInstanceBuilder(new NullLocator());
            var runner = new ScriptRunner(typeof(RunnerTests).Assembly.DefinedTypes, testInstanceBuilder, new TestStringBuilder());
            await runner.Run(script);
            return testInstanceBuilder;
        }
    }
}