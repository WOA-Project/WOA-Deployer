namespace Deployer.Core.Scripting.MicroParser
{
    public class Assignment
    {
        public string Identifier { get; }
        public object Value { get; }

        public Assignment(string identifier, object value)
        {
            Identifier = identifier;
            Value = value;
        }
    }
}