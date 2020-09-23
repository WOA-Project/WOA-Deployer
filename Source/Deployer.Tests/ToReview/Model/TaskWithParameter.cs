using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tests.Model
{
    public class TaskWithParameter : IDeploymentTask
    {
        public string Parameter { get; }

        public TaskWithParameter(string parameter)
        {
            Parameter = parameter;
        }

        public Task Execute()
        {
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"TaskWithParameter";
        }
    }
}