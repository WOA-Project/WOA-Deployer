using System.Diagnostics;
using System.Threading.Tasks;

namespace Deployer.Core
{
    class ShellOpen : IShellOpen
    {
        public Task Open(string filename)
        {
            var info = new ProcessStartInfo(filename)
            {
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
            };

            var process = Process.Start(info);
            
            process?.WaitForExit();

            return Task.CompletedTask;
        }
    }
}