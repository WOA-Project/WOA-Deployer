using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Core;

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