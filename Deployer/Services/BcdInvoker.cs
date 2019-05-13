using System.Threading.Tasks;
using Deployer.Utils;

namespace Deployer.Services
{
    public class BcdInvoker : IBcdInvoker
    {
        private readonly string commonArgs;
        private readonly string bcdEdit;

        public BcdInvoker(string store)
        {
            bcdEdit = WindowsCommandLineUtils.BcdEdit;
            commonArgs = $@"/STORE ""{store}""";
        }

        public async Task<string> Invoke(string command)
        {
            var processResults = await ProcessUtils.RunProcessAsync(bcdEdit, $@"{commonArgs} {command}");
            var output = string.Join("\n", processResults.StandardOutput);
            var errors = string.Join("\n", processResults.StandardError);
            return string.Join(";", output, errors);
        }
    }
}