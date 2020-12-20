using System.Threading.Tasks;
using Deployer.Tools.AzureDevOps.ArtifactModel;

namespace Deployer.Tools.AzureDevOps
{
    public interface IAzureDevOpsBuildClient
    {
        Task<Artifact> ArtifactFromLatestBuild(string org, string project, int definitionId, string artifactName);
    }
}