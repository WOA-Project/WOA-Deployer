namespace Deployer.Ide
{
    public class IdeMessage
    {
        public string Message { get; }

        public IdeMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}