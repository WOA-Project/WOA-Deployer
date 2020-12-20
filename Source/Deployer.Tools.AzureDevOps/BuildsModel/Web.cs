using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class Web
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}