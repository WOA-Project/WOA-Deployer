using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Core.DevOpsBuildClient.ArtifactModel;
using Deployer.Core.DevOpsBuildClient.BuildsModel;
using Refit;

namespace Deployer.Core.DevOpsBuildClient
{
    public class AzureDevOpsBuildClient : IAzureDevOpsBuildClient
    {
        private readonly IBuildApiClient inner;

        public AzureDevOpsBuildClient(IBuildApiClient inner)
        {
            this.inner = inner;
        }

        public static AzureDevOpsBuildClient Create(Uri baseAddress)
        {
            var httpClient = new HttpClient(new CustomHttpClientHandler())
            {
                BaseAddress = baseAddress,                
            };
            
            return new AzureDevOpsBuildClient(RestService.For<IBuildApiClient>(httpClient));
        }

        public Task<Artifact> GetArtifact(string org, string project, int buildId, string artifactsName)
        {
            return inner.GetArtifact(org, project, buildId, artifactsName);
        }

        public async Task<IList<Build>> GetBuilds(string org, string project, int definition)
        {
            var builds = await inner.GetBuilds(org, project, definition);
            return builds.Value;
        }

        public async Task<Artifact> ArtifactFromLatestBuild(string org, string project, int definitionId, string artifactName)
        {
            var builds = await GetBuilds(org, project, definitionId);
            var latest = builds.OrderByDescending(x => x.Id).First();
            var artifact = await GetArtifact(org, project, latest.Id, artifactName);
            return artifact;
        }
    }
}