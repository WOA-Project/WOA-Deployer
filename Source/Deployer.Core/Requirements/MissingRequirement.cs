namespace Deployer.Core.Requirements
{
    public class MissingRequirement
    {
        public MissingRequirement(string key, RequirementKind kind, string description)
        {
            Key = key;
            Kind = kind;
            Description = description;
        }

        public string Key { get; }
        public RequirementKind Kind { get; }
        public string Description { get; }
    }
}