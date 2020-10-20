namespace Deployer.Core.Requirements
{
    public class ResolveSettings
    {
        public ResolveSettings(string key, RequirementKind kind)
        {
            Key = key;
            Kind = kind;
        }

        public string Key { get; }
        public RequirementKind Kind { get; }
    }
}