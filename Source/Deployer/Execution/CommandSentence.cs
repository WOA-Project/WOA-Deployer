namespace Deployer.Execution
{
    public class CommandSentence : Sentence
    {
        public Command Command { get; }

        public CommandSentence(Command command)
        {
            Command = command;
        }

        public override string ToString()
        {
            return $"{Command}";
        }
    }
}