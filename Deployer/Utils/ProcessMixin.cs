using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RunProcessAsTask;

namespace Deployer.Utils
{
    public static class ProcessMixin
    {
        public static async Task<ProcessResults> RunProcess(string fileName,
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