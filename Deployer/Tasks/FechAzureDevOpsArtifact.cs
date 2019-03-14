using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.DevOpsBuildClient;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from Azure DevOps: {0}")]
    public class FechAzureDevOpsArtifact : IDeploymentTask
    {
        private string org;
        private string project;
        private int definitionId;
        private string artifactName;
        private readonly IAzureDevOpsBuildClient buildClient;
        private readonly IZipExtractor extractor;
        private readonly IDownloader downloader;
        private readonly IDownloadProgress progressObserver;
        private string folderPath;

        private const string SubFolder = "Downloaded";

        public FechAzureDevOpsArtifact(string descriptor, IAzureDevOpsBuildClient buildClient, IZipExtractor extractor, IDownloader downloader, IDownloadProgress progressObserver)
        {
            ParseDescriptor(descriptor);

            this.buildClient = buildClient;
            this.extractor = extractor;
            this.downloader = downloader;
            this.progressObserver = progressObserver;
        }

        private void ParseDescriptor(string descriptor)
        {
            var parts = descriptor.Split(new[] { ";" }, StringSplitOptions.None);

            org = parts[0];
            project = parts[1];
            definitionId = int.Parse(parts[2]);
            artifactName = parts[3];
            folderPath = Path.Combine(SubFolder, artifactName);
        }

        public async Task Execute()
        {
            if (Directory.Exists(folderPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", artifactName);
                return;
            }

            var artifact = await buildClient.LatestBuildArtifact(org, project, definitionId, artifactName);

            var url = artifact.Resource.DownloadUrl;
            var stream = await downloader.GetStream(url, progressObserver);
            await extractor.ExtractFirstChildToFolder(stream, folderPath, progressObserver);
        }
    }
}