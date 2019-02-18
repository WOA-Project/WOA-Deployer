using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Zip Unpack from {0} to {1}")]
    public class ZipUnpack : IDeploymentTask
    {
        private readonly string url;
        private readonly string destination;
        private readonly IZipExtractor extractor;
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipUnpack(string url, string destination, IZipExtractor extractor, IFileSystemOperations fileSystemOperations)
        {
            this.url = url;
            this.destination = destination;
            this.extractor = extractor;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute()
        {

            if (fileSystemOperations.DirectoryExists(destination))
            {
                return;
            }

            using (var httpClient = new HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(url);
                
                await extractor.ExtractToFolder(stream, destination);
            }
        }
    }
}