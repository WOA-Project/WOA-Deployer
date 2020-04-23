using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class Badge
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}