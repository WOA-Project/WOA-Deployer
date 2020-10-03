using System.Collections.Generic;
using Deployer.Core.Compiler;
using FluentAssertions;
using Iridio;
using Iridio.Binding;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Parsing;
using Moq;
using Optional;
using Xunit;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Tests
{
    public class DeployerCompilerTests
    {
        [Theory]
        [InlineData("Main { }", "Main\r\n{\r\n\ttest = 123;\r\n}")]
        [InlineData("Main { }", "Main\r\n{\r\n\ttest = 123;\r\n}")]
        public void Injection(string original, string modified)
        {
            var preprocessor = new Mock<IPreprocessor>();
            preprocessor.Setup(p => p.Process(It.IsAny<string>())).Returns(original);
            var parser = new Parser();
            var binder = new Binder(new BindingContext(new List<IFunction>()));
            var sut = new DeployerCompiler(preprocessor.Object, parser, binder);
            var compilation = sut.Compile("fake.src", new[] { new Assignment("test", 123), });

            compilation
                .MapRight(unit => FormattingExtensions.AsString((IBoundNode) unit))
                .Should()
                .BeEquivalentTo(Either.Success<Errors, string>(modified), options => options.ComparingByMembers<Option<string>>());
        }
    }
}