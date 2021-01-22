namespace Deployer.Core.Requirements
{
    public class ResolveSettings
    {
        public ResolveSettings(string key, RequirementDefinition definition, string description)
        {
            Key = key;
            Definition = definition;
            Description = description;
        }

        public string Key { get; }
        public RequirementDefinition Definition { get; }
        public string Description { get; }
    }
}