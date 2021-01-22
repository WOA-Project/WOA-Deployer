namespace Deployer.Core.Requirements
{
    public class FulfilledRequirement
    {
        public FulfilledRequirement(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public RequirementDefinition Definition { get; }
        public object Value { get; }
    }
}