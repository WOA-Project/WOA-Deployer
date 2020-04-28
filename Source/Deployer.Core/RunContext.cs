using SimpleScript;

namespace Deployer.Core
{
    internal class RunContext
    {
        public Script Script { get; }
        public string WorkingDirectory { get; }

        public RunContext(Script script, string workingDirectory)
        {
            Script = script;
            WorkingDirectory = workingDirectory;
        }
    }
}