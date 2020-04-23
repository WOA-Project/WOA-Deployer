using Superpower;

namespace Deployer.Core.Scripting.MicroParser
{
    public static class Parser
    {
        public static Micro Parse(string source)
        {
            var tokenizer = Tokenizer.Create();
            return Parsers.Micro.Parse(tokenizer.Tokenize(source));
        }
    }
}