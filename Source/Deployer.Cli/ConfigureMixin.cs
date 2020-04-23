using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Linq;
using System.Web.Compilation;
using Deployer.Cli.Registrations;
using Deployer.Cli.Services;
using Deployer.Core;
using Grace.DependencyInjection;
using Serilog;
using SimpleScript;

namespace Deployer.Cli
{
    public static class ConfigureMixin
    {
        public static CommandLineBuilder Configure(this CommandLineBuilder builder, DependencyInjectionContainer container)
        {
            var option = new Option<Dictionary<string, object>>(
                "--variables",
                parseArgument: argResult => argResult
                    .Tokens
                    .Select(t => t.Value.Split('='))
                    .ToDictionary(p => p[0], p =>
                    {
                        if (int.TryParse(p[1], out var n))
                        {
                            return n;
                        }

                        return (object) p[1];
                    }));

            var deployCommand = new Command("deploy")
            {
                option,
                new Option<Device>("--device", argResult =>
                {
                    var fromString = Device.FromString(argResult.Tokens.First().Value);
                    return fromString;
                }),
                new Option<bool>(new[] {"-d", "--auto-detect"}, "Use device auto-detection")
            };

            deployCommand.Handler = CommandHandler.Create(async (bool autoDetect, Device device, IDictionary<string, object> variables) =>
            {
                container.Configure(x => x.ExportInstance(new DictionaryBasedSatisfier(variables)).As<IRequirementSatisfier>());
                var deployer = container.Locate<WoaDeployerConsole>();
                await deployer.Deploy(device, autoDetect);
            });

            builder.AddCommand(deployCommand);

            var listCommand = new Command("list")
            {
                new Argument<ListType>("listType")
            };

            listCommand.Handler = CommandHandler.Create((ListType listType) =>
            {
                container.Configure(x => x.ExportInstance(new DictionaryBasedSatisfier(new Dictionary<string, object>())).As<IRequirementSatisfier>());
                var deployer = container.Locate<WoaDeployerConsole>();
                deployer.ListFunctions();
            });
            
            builder.AddCommand(listCommand);
            return builder;
        }
    }

    public enum ListType
    {
        Functions
    }
}