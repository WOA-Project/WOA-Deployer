using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Deployer.Core.Exceptions;
using SimpleScript;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class DeviceDeployer : Deployer
    {
        private const string FeedFolder = "Feed";
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");
        private readonly ISubject<string> additionalMessages = new Subject<string>();
        private readonly IEnumerable<IContextualizer> contextualizers;

        public DeviceDeployer(ICompiler compiler, IRunner runner, IFileSystemOperations fileSystemOperations,
            IEnumerable<IContextualizer> contextualizers, IRequirementSatisfier requirementSatisfier) : base(runner, compiler, requirementSatisfier, fileSystemOperations)
        {
            this.contextualizers = contextualizers;
        }

        private async Task DownloadFeed()
        {
            await DeleteFeedFolder();
            await Run(BootstrapPath, new Dictionary<string, object>());
        }

        public async Task Deploy(string path, Device device)
        {
            await DownloadFeed();
            var variables = new Dictionary<string, object>();
            await ContextualizeFor(device, variables);
            await Run(Path.Combine(FeedFolder, path), variables);
            Message("Deployment successful");
        }

        private async Task DeleteFeedFolder()
        {
            if (FileSystemOperations.DirectoryExists(FeedFolder))
            {
                await FileSystemOperations.DeleteDirectory(FeedFolder);
            }
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
    }
}