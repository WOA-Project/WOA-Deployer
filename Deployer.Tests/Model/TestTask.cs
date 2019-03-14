using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tests.Model
{
    public class TestTask : IDeploymentTask
    {
        public Task Execute()
        {
            IsExecuted = true;
            return Task.CompletedTask;
        }

        public bool IsExecuted { get; set; }

        public override string ToString()
        {
            return "TestTask";
        }
    }
}