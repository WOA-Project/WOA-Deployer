using Newtonsoft.Json;

namespace Deployer.Core.DevOpsBuildClient.BuildsModel
{
    public class Plan
    {

        [JsonProperty("planId")]
        public string PlanId { get; set; }
    }
}