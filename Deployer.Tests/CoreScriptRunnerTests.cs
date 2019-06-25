using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class CoreScriptRunnerTests
    {
        [Fact]
        public async Task TestTask()
        {
            var createdTasks = await Run("TestTask");
            createdTasks.Should().Be("TestTask()");
        }

        [Fact]
        public async Task TestTaskWithParameter()
        {
            var createdTasks = await Run("TaskWithParameter \"hola\"");
            createdTasks.Should().Be("TaskWithParameter(\"hola\", {null})");
        }

        [Fact]
        public async Task TestTaskWithSpecialParameters()
        {
            var createdTasks = await Run("TaskWithParameter null true false");
            createdTasks.Should().Be("TaskWithParameter({null},{true},{false})");
        }

        private static async Task<string> Run(string script)
        {
            var testInstanceBuilder = new TestInstanceBuilder(new NullLocator());
            var runner = new ScriptRunner(typeof(CoreScriptRunnerTests).Assembly.DefinedTypes, testInstanceBuilder,
                new TestStringBuilder());
            
            await runner.Run(new ScriptParser(Tokenizer.Create()).Parse(script));
            return testInstanceBuilder.CreatedInstances;
        }
    }    
}

    