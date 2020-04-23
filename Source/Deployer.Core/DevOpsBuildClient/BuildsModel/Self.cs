using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class Self
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
