using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Console;
using Pastel;
using Serilog;
using Serilog.Events;
using SimpleScript;

namespace Deployer.Cli
{
    public class WoaDeployerConsole : IDisposable
    {
        private readonly WoaDeployer woaDeployer;
        private readonly IEnumerable<IDetector> detectors;
        private readonly IEnumerable<IFunction> functions;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ConsoleProgressUpdater updater;

        public WoaDeployerConsole(WoaDeployer woaDeployer, IEnumerable<IDetector> detectors,
            Func<ConsoleProgressUpdater> progressUpdater, IEnumerable<IFunction> functions)
        {
            this.woaDeployer = woaDeployer;
            this.detectors = detectors;
            this.functions = functions;
            updater = progressUpdater();
            woaDeployer.Messages.Subscribe(Log.Information).DisposeWith(disposables);

            ConfigureLogging();
        }

        private static void ConfigureLogging()
        {
            var template = "{Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(@"Logs\Log-{Date}.txt")
                .WriteTo.Console(LogEventLevel.Information, template)
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        public void Dispose()
        {
            disposables?.Dispose();
            updater?.Dispose();
        }

        public async Task Deploy(Device device, bool autoDetect)
        {
            DisplayInfo();

            device = await GetDevice(device, autoDetect);

            try
            {
                Log.Information("Starting deployment for {Device}...", device);
                await woaDeployer.Deploy(device);
                Log.Information("Deployment finished");
            }
            catch (RequirementException re)
            {
                Log.Error($"The script requires the following variables to be defined:\n{string.Join("\n", re.Requirements.Select(s => "-" + s))}\n\nPlease, specify them using the --variables argument");
            }
        }

        public void ListFunctions()
        {
            DisplayInfo();

            Console.WriteLine("\nFunction list:");
            functions.OrderBy(x => x.Name).ToList().ForEach(PrettyPrint);
        }

        private static void PrettyPrint(IFunction function)
        {
            var accent1 = Color.DeepSkyBlue;
            var accent2 = Color.Aqua;
            var accent3 = Color.White;
            var error = Color.Red;

            var parameterList = function.Arguments.Select(info =>
            {
                var typeName = TokenizeTypeName(info.Type.Name);
                var name = info.Name;
                return $"{typeName.Pastel(accent2)} {name.Pastel(accent1)}";
            });

            var signature = $"{string.Join(", ".Pastel(accent3), parameterList)}";
            
            string returnStr;
            if (function.ReturnType == typeof(Task))
            {
                returnStr = "";
            }
            else
            {
                var name = function.ReturnType.GenericTypeArguments.FirstOrDefault()?.Name;
                if (name == null)
                {
                    returnStr = "Unknown".Pastel(error);
                }
                else
                {
                    returnStr = TokenizeTypeName(name).Pastel(accent1);
                }
            }

            var functionName = function.Name.Pastel(accent3);
            var text = $"\t{returnStr}\t{functionName}({signature})";

            Console.WriteLine(text);
        }

        private  static string TokenizeTypeName(string name)
        {
            var primitive = name.ToLower();
            switch (primitive)
            {
                case "int32":
                    return "int";
            }

            return primitive;
        }

        private async Task<Device> GetDevice(Device device, bool autoDetect)
        {
            if (autoDetect)
            {
                var detections = await Task.WhenAll(detectors.Select(detector => detector.Detect()));
                var detectedDevice = detections.FirstOrDefault();
                device = detectedDevice;

                if (detectedDevice != null)
                {
                    Log.Information("Detected device: {Device}", device);
                }
            }

            if (device == null)
            {
                throw new UndeterminedDeviceException();
            }

            return device;
        }

        private static void DisplayInfo()
        {
            Log.Information("WOA Deployer v{AppVersionMixin.VersionString}");
        }
    }
}