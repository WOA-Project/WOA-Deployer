using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RunProcessAsTask;
using Serilog;

namespace Deployer.Utils
{
    public static class ProcessUtils
    {
        public static ProcessResults Run(string command, string arguments)
        {
            Log.Verbose("Starting process {@Process}", new { command, arguments });
            var processResults = RunProcessAsync(command, arguments).Result;
            Log.Verbose("Process output {Output} {Errors}", processResults.StandardOutput, processResults.StandardOutput);

            return processResults;
        }


        public static async Task<ProcessResults> RunProcessAsync(string fileName,
            string args = "", IObserver<string> outputObserver = null, IObserver<string> errorObserver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            return await processStartInfo.RunAsync(outputObserver, errorObserver, cancellationToken);
        }
    }
}