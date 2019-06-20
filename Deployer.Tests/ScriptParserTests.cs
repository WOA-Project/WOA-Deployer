using Deployer.Execution;
using FluentAssertions;
using Xunit;

namespace Deployer.Tests
{
    public class ScriptParserTests
    {
        [Theory]
        // Base tests
        [InlineData("Task", "Task()")]
        [InlineData("Task \"Hola\"", "Task(Hola)")]
        [InlineData("Task \"Hola\" \"Cómo vas\"", "Task(Hola,Cómo vas)")]
        [InlineData("# Comment", "# Comment")]
        // New lines related tests
        [InlineData("\nTask", "Task()")]
        [InlineData(" \nTask", "Task()")]
        [InlineData("\r\nTask", "Task()")]
        [InlineData(" \r\nTask", "Task()")]
        [InlineData("Task\n", "Task()")]
        [InlineData("Task\n ", "Task()")]
        [InlineData("Task\r\n", "Task()")]
        [InlineData("Task\r\n ", "Task()")]
        [InlineData("Task\n\n", "Task()")]
        [InlineData("Task\n\n ", "Task()")]
        [InlineData("Task\r\n\r\n", "Task()")]
        [InlineData("Task\r\n\r\n ", "Task()")]
        [InlineData("Task\r\n\r\n\r\n", "Task()")]
        [InlineData("Task\r\n\r\n\r\n ", "Task()")]
        [InlineData("Task\r\n \r\n", "Task()")]
        [InlineData("Task\r\n \r\n ", "Task()")]
        [InlineData("Task1\r\n \r\nTask2", "Task1()\nTask2()")]
        [InlineData("Task1 \r\n \r\n Task2", "Task1()\nTask2()")]
        [InlineData("Task1\r\n\r\n\r\nTask2", "Task1()\nTask2()")]
        [InlineData("Task1 \r\n \r\n \r\n Task2", "Task1()\nTask2()")]
        public void Test_Parse(string input, string expected)
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
