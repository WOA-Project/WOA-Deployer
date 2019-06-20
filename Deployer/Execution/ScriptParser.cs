using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Superpower;

namespace Deployer.Execution
{
    public class ScriptParser : IScriptParser
    {
        private static Regex _emptyLinesRegex = new Regex(@"((\s*\r?\n)+\s*){2,}");
        private readonly Tokenizer<LangToken> tokenizer;

        public ScriptParser(Tokenizer<LangToken> tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public Script Parse(string input)
        {
            input = _emptyLinesRegex.Replace(input.Trim(), "\n");

            var tokenList = tokenizer.Tokenize(input);
            return Parsers.Script.Parse(tokenList);
        }
    }
}