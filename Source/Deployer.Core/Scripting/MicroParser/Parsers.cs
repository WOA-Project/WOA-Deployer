using Iridio.Parsing;
using Superpower;
using Superpower.Parsers;

namespace Deployer.Core.Scripting.MicroParser
{
    public class Parsers
    {
        private static readonly TokenListParser<MicroToken, string> Identifier = Token.EqualTo(MicroToken.Identifier).Select(x => x.ToStringValue());

        private static readonly TokenListParser<MicroToken, string> Text = Token.EqualTo(MicroToken.Text)
            .Apply(ExtraParsers.SpanBetween('\'').Select(x => x.ToStringValue()));

        private static readonly TokenListParser<MicroToken, int> Number = Token.EqualTo(MicroToken.Number).Apply(Numerics.IntegerInt32);

        private static readonly TokenListParser<MicroToken, object> Value = Number.Select(i => (object)i).Or(Text.Select(s => (object)s));

        private static readonly TokenListParser<MicroToken, Assignment> Assignment = from identifier in Identifier
            from eq in Token.EqualTo(MicroToken.Equal)
            from v in Value.OptionalOrDefault()
            select new Assignment(identifier, v);

        public static readonly TokenListParser<MicroToken, Micro> Micro = Assignment.ManyDelimitedBy(Token.EqualTo(MicroToken.Comma))
            .Select(assignments => new Micro(assignments));
    }
}