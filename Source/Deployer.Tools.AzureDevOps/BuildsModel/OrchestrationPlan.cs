using Newtonsoft.Json;

namespace Deployer.Tools.AzureDevOps.BuildsModel
{
    public class OrchestrationPlan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}