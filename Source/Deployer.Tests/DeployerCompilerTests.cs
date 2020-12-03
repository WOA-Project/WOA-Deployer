using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(Data))]
        public void Injection(string original, IDictionary<string, object> assignments, string modified)
        {
            var preprocessor = new Mock<IPreprocessor>();
            preprocessor.Setup(p => p.Process(It.IsAny<string>())).Returns(original);
            var parser = new Parser();
            var binder = new Binder(new List<IFunctionDeclaration>());
            var sut = new DeployerCompiler(preprocessor.Object, parser, binder);
            var compilation = sut.Compile("fake.src", assignments.Select(pair => new Assignment(pair.Key, pair.Value)));

            compilation
                .MapRight(unit => FormattingExtensions.AsString((IBoundNode) unit))
                .Should()
                .BeEquivalentTo(Either.Success<Errors, string>(modified), options => options.ComparingByMembers<Option<string>>());
        }

        public static IEnumerable<object[]> Data()
        {
            yield return new object[] {"Main { }", new Dictionary<string, object> {{"test", 123}}, "Main\r\n{\r\n\ttest = 123;\r\n}"};
            yield return new object[] {"Main { }", new Dictionary<string, object> {{"test", "salute"}}, "Main\r\n{\r\n\ttest = \"salute\";\r\n}"};
        }
    }
}