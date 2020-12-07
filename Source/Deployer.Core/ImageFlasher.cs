using System;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.FileSystem;
using Deployer.Core.Services;
using Deployer.Core.Utils;
using Serilog;
using Zafiro.Core;

namespace Deployer.NetFx
{
    public class ImageFlasher : IImageFlasher
    {
        private readonly Regex percentRegex = new Regex(@"(\d*.\d*)%");

        public string EtcherPath
        {
            get
            {
                var platformSuffix = Environment.Is64BitProcess ? "x64" : "x86";
                var etcherPath = Path.Combine("Core", "Tools", platformSuffix, "Etcher-Cli", "Etcher");
                return etcherPath;
            }
        }

        public async Task Flash(IDisk disk, string imagePath, IOperationProgress progressObserver = null)
        {
            Log.Information("Flashing GPT image...");
            
            ISubject<string> outputSubject = new Subject<string>();
            IDisposable stdOutputSubscription = null;
            bool isValidating = false;
            if (progressObserver != null)
            {
                stdOutputSubscription = outputSubject
                    .Do(s =>
                    {
                        if (!isValidating && CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, "validating", 0, CompareOptions.IgnoreCase) != -1)
                        {
                            progressObserver?.Percentage.OnNext(double.NaN);
                            Log.Information("Validating flashed image...");                            
                            isValidating = true;
                        }                        
                    })
                    .Select(GetPercentage)
                    .Where(d => !double.IsNaN(d))
                    .Subscribe(progressObserver.Percentage);
            }
            
            var args = $@"-d \\.\PHYSICALDRIVE{disk.Number} ""{imagePath}"" --yes --no-unmount";
            Log.Verbose("We are about to run Etcher: {ExecName} {Parameters}", EtcherPath, args);
            var processResults = await ProcessMixin.RunProcess(EtcherPath, args, outputObserver: outputSubject);
            if (processResults.ExitCode != 0)
            {
                Log.Error("Cannot flash the image with Etcher. Execution results: {Results}", processResults);
                throw new FlashException($"Cannot flash the image: {imagePath} to {disk}");
            }

            progressObserver?.Percentage.OnNext(double.NaN);

            stdOutputSubscription?.Dispose();

            await disk.Refresh();

            await EnsureDiskHasNewGuid(disk);

            Log.Information("GPT image flashed");
        }

        private static async Task EnsureDiskHasNewGuid(IDisk disk)
        {
            await disk.SetGuid(Guid.NewGuid());
        }

        private double GetPercentage(string output)
        {
            if (output == null)
            {
                return double.NaN;
            }

            var matches = percentRegex.Match(output);

            if (matches.Success)
            {
                var value = matches.Groups[1].Value;
                try
                {
                    var percentage = double.Parse(value, CultureInfo.InvariantCulture) / 100D;
                    return percentage;
                }
                catch (FormatException)
                {
                    Log.Warning($"Cannot convert {value} to double");
                }
            }

            return double.NaN;
        }
    }
}