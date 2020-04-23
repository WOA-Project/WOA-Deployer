using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;

namespace Deployer.Cli.Services
{
    public class DictionaryBasedSatisfier : IRequirementSatisfier
    {
        private System.Collections.Generic.IDictionary<string, object> variables;

        public DictionaryBasedSatisfier(IDictionary<string, object> variables)
        {
            this.variables = variables;
        }

        public Task<bool> Satisfy(IDictionary<string, object> unsatisfied)
        {
            foreach (var requirement in unsatisfied.Keys.ToList())
            {
                if (variables.TryGetValue(requirement, out var value))
                {
                    unsatisfied[requirement] = value;
                }
            }

            return Task.FromResult(true);
        }
    }
}