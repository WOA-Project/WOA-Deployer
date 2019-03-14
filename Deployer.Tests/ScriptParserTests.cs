using Deployer.Execution;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class ScriptParserTests
    {
        [Theory]
        [InlineData("Task", "Task()")]
        [InlineData("Task \"Hola\"", "Task(Hola)")]
        [InlineData("Task \"Hola\" \"Cómo vas\"", "Task(Hola,Cómo vas)")]
        [InlineData("# Comment", "# Comment")]
        public void Test1(string input, string expected)
        {
            AssertParse(input, expected);
        }

        private void AssertParse(string input, string expected)
        {
            var parser = new ScriptParser(Tokenizer.Create());
            parser.Parse(input).ToString().Should().Be(expected);        
        }
    }
}
