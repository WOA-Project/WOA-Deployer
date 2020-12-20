using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.ArtifactModel
{
    public class Properties
    {

        [JsonProperty("localpath")]
        public string Localpath { get; set; }
    }
}
