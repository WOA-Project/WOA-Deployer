namespace Deployer.Tests
{
    public class FullFilledRequirement
    {
        public FullFilledRequirement(string variableName, string kind, string value)
        {
            VariableName = variableName;
            Kind = kind;
            Value = value;
        }

        public string VariableName { get; }
        public string Kind { get; }
        public string Value { get; }
    }
}