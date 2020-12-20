using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class Timeline
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}