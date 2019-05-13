using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.DevOpsBuildClient;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from Azure DevOps: {0}")]
    public class FetchAzureDevOpsArtifact : DownloaderTask
    {
        private readonly IAzureDevOpsBuildClient buildClient;
        private readonly IDownloader downloader;
        private readonly IZipExtractor extractor;
        private readonly IOperationProgress progressObserver;
        private string artifactName;
        private int definitionId;
        private string org;
        private string project;

        public FetchAzureDevOpsArtifact(string descriptor, IAzureDevOpsBuildClient buildClient, IZipExtractor extractor,
            IDownloader downloader, IOperationProgress progressObserver, IDeploymentContext deploymentContext) : 
            base(deploymentContext)
        {
            ParseDescriptor(descriptor);

            this.buildClient = buildClient;
            this.extractor = extractor;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
        }

        public override string ArtifactPath => Path.Combine(AppPaths.ArtifactDownload, artifactName);

        private void ParseDescriptor(string descriptor)
        {
            var parts = descriptor.Split(new[] {";"}, StringSplitOptions.None);

            org = parts[0];
            project = parts[1];
            definitionId = int.Parse(parts[2]);
            artifactName = parts[3];
        }

        protected override async Task ExecuteCore()
        {
            if (Directory.Exists(ArtifactPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", artifactName);
                return;
            }

            var artifact = await buildClient.LatestBuildArtifact(org, project, definitionId, artifactName);

            var url = artifact.Resource.DownloadUrl;
            var stream = await downloader.GetStream(url, progressObserver);
            await extractor.ExtractFirstChildToFolder(stream, ArtifactPath, progressObserver);

            SaveMetadata(artifact);
        }
    }
}