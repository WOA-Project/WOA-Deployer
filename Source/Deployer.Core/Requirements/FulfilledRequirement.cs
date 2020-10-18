namespace Deployer.Core.Requirements
{
    public class FulfilledRequirement
    {
        public FulfilledRequirement(string key, RequirementKind kind, object value)
        {
            Key = key;
            Kind = kind;
            Value = value;
        }

        public string Key { get; }
        public RequirementKind Kind { get; }
        public string Description { get; }
        public object Value { get; }
    }
}