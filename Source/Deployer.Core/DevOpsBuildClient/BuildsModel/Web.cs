using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class Web
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}