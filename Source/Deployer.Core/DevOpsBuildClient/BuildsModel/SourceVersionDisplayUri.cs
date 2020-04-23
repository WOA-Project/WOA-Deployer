using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class SourceVersionDisplayUri
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}