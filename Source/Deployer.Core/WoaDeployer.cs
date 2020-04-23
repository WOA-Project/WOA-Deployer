using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Exceptions;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Utils;
using SimpleScript;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class WoaDeployer
    {
        private const string MainScriptName = "Main.txt";
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");
        private static readonly string ScriptsDownloadPath = Path.Combine(AppPaths.Feed, "Scripts");

        private readonly ICompiler compiler;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IEnumerable<IContextualizer> contextualizers;
        private readonly IRequirementSatisfier requirementSatisfier;
        private readonly IRunner runner;

        public WoaDeployer(ICompiler compiler, IRunner runner, IFileSystemOperations fileSystemOperations,
            IEnumerable<IContextualizer> contextualizers, IRequirementSatisfier requirementSatisfier)
        {
            this.compiler = compiler;
            this.runner = runner;
            this.fileSystemOperations = fileSystemOperations;
            this.contextualizers = contextualizers;
            this.requirementSatisfier = requirementSatisfier;
        }

        public IObservable<string> Messages => runner.Messages;

        public async Task Deploy(Device device)
        {
            var variables = new Dictionary<string, object>();
            await ContextualizeFor(device, variables);
            var context = await GetScriptContext(device);
            await Run(context, variables);
        }

        public async Task<IEnumerable<string>> GetRequirements(Device device)
        {
            var context = await GetScriptContext(device);
            return GetRequirements(context.Script);
        }

        private async Task DownloadDeploymentScripts()
        {
            if (fileSystemOperations.DirectoryExists(AppPaths.Feed))
            {
                await fileSystemOperations.DeleteDirectory(AppPaths.Feed);
            }

            var script = compiler.Compile(BootstrapPath);
            var workingDir = Path.GetDirectoryName(BootstrapPath);
            var scriptContext = new ScriptContext(script, workingDir);

            await Run(scriptContext, new Dictionary<string, object>());
        }

        private async Task Run(ScriptContext scriptContext, IDictionary<string, object> variables)
        {
            using (new DirectorySwitch(fileSystemOperations, scriptContext.WorkingDirectory))
            {
                await SatisfyRequirements(scriptContext.Script, variables);
                await runner.Run(scriptContext.Script, variables);
            }
        }

        private async Task SatisfyRequirements(Script script, IDictionary<string, object> variables)
        {
            var requirements = GetRequirements(script);

            var defined = variables.Select(pair => pair.Key);
            var unsatisfied = requirements.Except(defined).ToList();
            var pending = unsatisfied.ToDictionary(s => s, s => (object)null);

            if (pending.Values.All(o => o != null))
            {
                return;
            }

            if (!await requirementSatisfier.Satisfy(pending))
            {
                throw new DeploymentCancelledException();
            }
            
            variables.AddRange(pending);

            if (pending.Any(x => x.Value is null))
            {
                throw new RequirementException(unsatisfied);
            }
        }

        private static IEnumerable<string> GetRequirements(Script script)
        {
            return script.Declarations
                .Where(tuple => tuple.Identifier == MetadataKey.Requirement)
                .Select(tuple => tuple.Value)
                .Distinct();
        }

        private async Task ContextualizeFor(Device device, IDictionary<string, object> variables)
        {
            var capableContextualizer = contextualizers.FirstOrDefault(x => x.CanContextualize(device));

            if (capableContextualizer is null)
            {
                return;
            }

            if (capableContextualizer is null)
            {
                throw new DeploymentException($"Cannot contextualize for this device: {device}");
            }

            await capableContextualizer.Setup(variables);
        }

        private async Task<ScriptContext> GetScriptContext(Device device)
        {
            await DownloadDeploymentScripts();
            var paths = new[] { ScriptsDownloadPath }.Concat(device.Identifier).Concat(new[] { MainScriptName });
            var scriptPath = Path.Combine(paths.ToArray());

            if (!fileSystemOperations.FileExists(scriptPath))
            {
                throw new DeploymentException($"Unsupported device {device}. The required script isn't present");
            }

            var script = compiler.Compile(scriptPath);
            var workingDirectory = Path.GetDirectoryName(scriptPath);
            return new ScriptContext(script, workingDirectory);
        }
    }

    public interface IRequirementSatisfier
    {
        Task<bool> Satisfy(IDictionary<string, object> unsatisfied);
    }

    internal class ScriptContext
    {
        public Script Script { get; }
        public string WorkingDirectory { get; }

        public ScriptContext(Script script, string workingDirectory)
        {
            Script = script;
            WorkingDirectory = workingDirectory;
        }
    }
}