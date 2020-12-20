using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class SourceVersionDisplayUri
    {

        [JsonProperty("href")]
        public string Href { get; set; }
    }
}