using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Tests
{
    public class TestPrompt
    {
        public Task<Option> PickOptions(string markdown, IEnumerable<Option> options)
        {
            return Task.FromResult((Option)null);
        }
    }
}