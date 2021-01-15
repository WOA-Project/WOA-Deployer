using System.Threading.Tasks;
using Deployer.Tools.Common;
using Serilog;

namespace Deployer.Tools.Bcd
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
            var processResults = await ProcessMixin.RunProcess(bcdEdit, $@"{commonArgs} {command}");
            var output = string.Join("\n", processResults.StandardOutput);
            var errors = string.Join("\n", processResults.StandardError);
            var result = string.Join(";", output, errors);
            Log.Information($"BCD Edit Command: '{command}'. Result: {result}");
            return result;
        }
    }
}