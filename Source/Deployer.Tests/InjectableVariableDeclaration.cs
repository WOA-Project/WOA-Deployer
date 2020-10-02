namespace Deployer.Tests
{
    public class InjectableVariableDeclaration
    {
        public InjectableVariableDeclaration(string name, object value)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; }
        public object Value { get; }
    }
}