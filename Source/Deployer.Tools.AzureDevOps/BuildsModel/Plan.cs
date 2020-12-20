using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class Plan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}