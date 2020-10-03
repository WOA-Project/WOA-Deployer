namespace Deployer.Core.Compiler
{
    public class Assignment
    {
        public Assignment(string name, object value)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; }
        public object Value { get; }
    }
}