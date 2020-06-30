using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private static readonly string PackagesPath = "Packages";
        private readonly ISubject<string> additionalMessages = new Subject<string>();

        private readonly ICompiler compiler;
        private readonly IEnumerable<IContextualizer> contextualizers;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IRequirementSatisfier requirementSatisfier;

        private readonly IRunner runner;

        public IObservable<string> Messages => runner.Messages.Merge(additionalMessages);

        public WoaDeployer(ICompiler compiler, IRunner runner, IFileSystemOperations fileSystemOperations,
            IEnumerable<IContextualizer> contextualizers, IRequirementSatisfier requirementSatisfier)
        {
            this.compiler = compiler;
            this.runner = runner;
            this.fileSystemOperations = fileSystemOperations;
            this.contextualizers = contextualizers;
            this.requirementSatisfier = requirementSatisfier;
        }

        public async Task RunScript(string path)
        {
            var runContext = Load(path);
            await Run(runContext, new Dictionary<string, object>());
            Message("Script execution finished");
        }

        public async Task Deploy(Device device)
        {
            var variables = new Dictionary<string, object>();
            await ContextualizeFor(device, variables);
            var context = await Load(device);
            await Run(context, variables);
            Message("Deployment successful");
        }

        private async Task DeletePackagesFolder()
        {
            if (fileSystemOperations.DirectoryExists(PackagesPath))
            {
                await fileSystemOperations.DeleteDirectory(PackagesPath);
            }
        }

        private void Message(string message)
        {
            additionalMessages.OnNext(message);
        }

        public async Task<IEnumerable<string>> GetRequirements(Device device)
        {
            var runContext = await Load(device);
            return GetRequirements(runContext.Script);
        }

        private Task DownloadFeed()
        {
            return Run(Load(BootstrapPath), new Dictionary<string, object>());
        }

        private async Task Run(RunContext runContext, IDictionary<string, object> variables)
        {
            using (new DirectorySwitch(fileSystemOperations, runContext.WorkingDirectory))
            {
                await DeletePackagesFolder();
                Message("Satisfying script requirements");
                await SatisfyRequirements(runContext.Script, variables);
                await runner.Run(runContext.Script, variables);
            }
        }

        private async Task SatisfyRequirements(Script script, IDictionary<string, object> variables)
        {
            var requirements = GetRequirements(script);

            var defined = variables.Select(pair => pair.Key);
            var unsatisfied = requirements.Except(defined).ToList();
            var pending = unsatisfied.ToDictionary(s => s, s => (object) null, StringComparer.InvariantCultureIgnoreCase);

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

        private async Task<RunContext> Load(Device device)
        {
            await DownloadFeed();
            var paths = new[] {PackagesPath}.Concat(device.Identifier).Concat(new[] {MainScriptName});
            var scriptPath = Path.Combine(paths.ToArray());

            if (!fileSystemOperations.FileExists(scriptPath))
            {
                throw new DeploymentException($"Unsupported device {device}. The required script isn't present");
            }

            return Load(scriptPath);
        }

        private RunContext Load(string scriptPath)
        {
            var script = compiler.Compile(scriptPath);
            var workingDirectory = Path.GetDirectoryName(scriptPath);
            return new RunContext(script, workingDirectory);
        }
    }
}