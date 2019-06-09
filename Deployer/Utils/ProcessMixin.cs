using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RunProcessAsTask;
using Serilog;
using Zafiro.Core;

namespace Deployer.Utils
{
    public static class ProcessMixin
    {
        public static async Task<ProcessResults> RunProcess(string fileName,
            string args = "", IObserver<string> outputObserver = null, IObserver<string> errorObserver = null, string workingDirectory = "",
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory,
            };

            var logInfo = new
            {
                Command = fileName,
                Arguments = args,
                workingDirectory,
            };

            Log.Debug("Running process: {@Info}", logInfo);
            var processResults = await processStartInfo.RunAsync(outputObserver, errorObserver, cancellationToken);
            var resultInfo = new
            {
                processResults.ExitCode,
                OutputOutput = processResults.StandardError.Join(),
                ErrorOutput = processResults.StandardError.Join(),
            };

            Log.Debug("End of process. Execution summary: {@Results}", resultInfo);

            return processResults;
        }
    }
}