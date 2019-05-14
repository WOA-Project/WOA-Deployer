using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Deployer.DevOpsBuildClient;
using Deployer.Execution.Testing;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Real.Tasks.Azure
{
    public class AzureDevOpsUnpackTests
    {
        [Fact]
        public async Task Test()
        {
            var zipExtractor = new ZipExtractor(new FileSystemOperations());
            var azureDevOpsClient = AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com"));
            using (var httpClient = new HttpClient())
            {
                IDownloader downloader = new Downloader(httpClient);
                var task = new FetchAzureDevOpsArtifact("LumiaWOA;Lumia950XLPkg;1;MSM8994 UEFI (Lumia 950 XL)", azureDevOpsClient, zipExtractor, downloader, null, new TestDeploymentContext(), new TestFileSystemOperations());
                await task.Execute();
            }
        }
    }
}