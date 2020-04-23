using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class Timeline
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}