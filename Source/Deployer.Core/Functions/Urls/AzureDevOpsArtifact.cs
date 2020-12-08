using System.Threading.Tasks;
using Deployer.Core.DevOpsBuildClient;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions.Urls
{
    public class AzureDevOpsArtifact : DeployerFunction
    {
        private readonly IAzureDevOpsBuildClient buildClient;

        public AzureDevOpsArtifact(IAzureDevOpsBuildClient buildClient,  IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.buildClient = buildClient;
        }

        public async Task<string> Execute(string org, string project, int buildDefinitionId, string artifactName)
        {
            var artifact = await buildClient.ArtifactFromLatestBuild(org, project, buildDefinitionId, artifactName);
            var url = artifact.Resource.DownloadUrl;
            return url;
        }
    }
}