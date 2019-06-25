using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tests.Model
{
    public class TaskWithParameter : IDeploymentTask
    {
        public object[] Parameters { get; }

        public TaskWithParameter(string parameter)
        {
            Parameters = new string[] { parameter };
        }

        public TaskWithParameter(string p1, string p2)
        {
            Parameters = new string[] { p1, p2 };
        }

        public TaskWithParameter(string p1, bool p2, bool p3)
        {
            Parameters = new object[] { p1, p2, p3 };
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