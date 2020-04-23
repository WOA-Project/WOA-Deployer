using SimpleScript.Ast;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Deployer.Core.Scripting.MicroParser
{
    public class Tokenizer
    {
        public static Tokenizer<MicroToken> Create()
        {
            return new TokenizerBuilder<MicroToken>()
                .Ignore(Character.WhiteSpace)
                .Match(ExtraParsers.SpanBetween('\''), MicroToken.Text)
                .Match(Character.EqualTo(','), MicroToken.Comma)
                .Match(Character.EqualTo('='), MicroToken.Equal)
                .Match(Numerics.Integer, MicroToken.Number)
                .Match(Span.Regex(@"\w+[\d\w_]*"), MicroToken.Identifier)
                .Build();
        }
    }
}