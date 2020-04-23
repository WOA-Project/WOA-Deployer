using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.ArtifactModel
{
    public class Properties
    {

        [JsonProperty("localpath")]
        public string Localpath { get; set; }
    }
}
