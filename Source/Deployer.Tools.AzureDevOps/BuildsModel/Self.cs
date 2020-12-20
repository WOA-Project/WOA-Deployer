using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class Self
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
