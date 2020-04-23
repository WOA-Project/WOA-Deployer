using System.Threading.Tasks;
using Deployer.Core.DevOpsBuildClient.ArtifactModel;

namespace Deployer.Core.DevOpsBuildClient
{
    public interface IAzureDevOpsBuildClient
    {
        Task<Artifact> ArtifactFromLatestBuild(string org, string project, int definitionId, string artifactName);
    }
}