using System;
using System.Threading.Tasks;
using Deployer.Services;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestBcdInvoker : IBcdInvoker
    {
        public async Task<string> Invoke(string command)
        {
            Log.Verbose("Invoked BCDEdit: '{Command}'", command);
            if (command.Contains("/create"))
                return Guid.NewGuid().ToString();

            return $"Executed '{command}'";
        }
    }
}