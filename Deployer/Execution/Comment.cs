namespace Deployer.Execution
{
    public class Comment : Sentence
    {
        public string Text { get; }

        public Comment(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return "# " + Text;
        }
    }
}