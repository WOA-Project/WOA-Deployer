using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class Badge
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}