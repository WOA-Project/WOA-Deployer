namespace Deployer.Tests
{
    public class FulfilledRequirement
    {
        public FulfilledRequirement(string variableName, RequirementKind kind, object value)
        {
            VariableName = variableName;
            Kind = kind;
            Value = value;
        }

        public string VariableName { get; }
        public RequirementKind Kind { get; }
        public object Value { get; }
    }
}