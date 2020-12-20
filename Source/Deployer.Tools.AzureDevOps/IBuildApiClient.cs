using System.Threading.Tasks;
using Deployer.Tools.AzureDevOps.ArtifactModel;
using Deployer.Tools.AzureDevOps.BuildsModel;
using Refit;

namespace Deployer.Tools.AzureDevOps
{
    public interface IBuildApiClient
    {
        [Get("/{org}/{project}/_apis/build/builds/{buildId}/artifacts?artifactName={artifactName}&api-version=5.0-preview.5")]
        Task<Artifact> GetArtifact(string org, string project, int buildId, string artifactName);

        [Get("/{org}/{project}/_apis/build/builds?definitions={definition}&sourceBranch={sourceBranch}")]
        Task<Builds> GetBuilds(string org, string project, int definition, string sourceBranch = "master");
    }
}