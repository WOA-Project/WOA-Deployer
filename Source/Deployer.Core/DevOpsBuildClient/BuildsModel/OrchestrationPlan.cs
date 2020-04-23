using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class OrchestrationPlan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}