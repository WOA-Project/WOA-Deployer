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
        private const string FeedFolder = "Feed";
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");
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
            await Run(path, new Dictionary<string, object>());
            Message("Script execution finished");
        }

        private async Task DownloadFeed()
        {
            await DeleteFeedFolder();
            await Run(BootstrapPath, new Dictionary<string, object>());
        }

        public async Task Deploy(Deployment deployment)
        {
            await DownloadFeed();
            var variables = new Dictionary<string, object>();
            //await ContextualizeFor(device, variables);
            await Run(deployment.ScriptPath, variables);
            Message("Deployment successful");
        }

        private void Message(string message)
        {
            additionalMessages.OnNext(message);
        }

        //public async Task<IEnumerable<string>> GetRequirements(Device device)
        //{
        //    var runContext = await Load(device);
        //    return GetRequirements(runContext.Script);
        //}

        //private async Task DownloadFeed()
        //{
        //    await DeleteFeedFolder();
        //    await Run(Load(BootstrapPath), new Dictionary<string, object>());
        //}

        private async Task DeleteFeedFolder()
        {
            if (fileSystemOperations.DirectoryExists(FeedFolder))
            {
                await fileSystemOperations.DeleteDirectory(FeedFolder);
            }
        }

        private async Task Run(string path, IDictionary<string, object> variables)
        {
            var script = compiler.Compile(path);
            var workingDirectory = Path.GetDirectoryName(path);

            using (new DirectorySwitch(fileSystemOperations, workingDirectory))
            {
                Message("Satisfying script requirements");
                await SatisfyRequirements(script, variables);
                await runner.Run(script, variables);
            }
        }

        private async Task SatisfyRequirements(Script script, IDictionary<string, object> variables)
        {
            var requirements = GetRequirements(script);

            var defined = variables.Select(pair => pair.Key);
            var unsatisfied = requirements.Except(defined).ToList();
            var pending = unsatisfied.ToDictionary(s => s, s => (object)null, StringComparer.InvariantCultureIgnoreCase);

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

        //private async Task ContextualizeFor(Device device, IDictionary<string, object> variables)
        //{
        //    var capableContextualizer = contextualizers.FirstOrDefault(x => x.CanContextualize(device));

        //    if (capableContextualizer is null)
        //    {
        //        return;
        //    }

        //    if (capableContextualizer is null)
        //    {
        //        throw new DeploymentException($"Cannot contextualize for this device: {device}");
        //    }

        //    await capableContextualizer.Setup(variables);
        //}

        //private async Task<RunContext> Load(Device device)
        //{
        //    await DownloadFeed();
        //    var paths = new[] {"Feed", }.Concat(device.Identifier).Concat(new[] {MainScriptName});
        //    var scriptPath = Path.Combine(paths.ToArray());

        //    if (!fileSystemOperations.FileExists(scriptPath))
        //    {
        //        throw new DeploymentException($"Unsupported device {device}. The required script isn't present");
        //    }

        //    return Load(scriptPath);
        //}

        private RunContext RunContextFrom(string scriptPath)
        {
            var script = compiler.Compile(scriptPath);
            var workingDirectory = Path.GetDirectoryName(scriptPath);
            return new RunContext(script, workingDirectory);
        }
    }
}