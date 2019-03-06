using System;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching zip from {0}")]
    public class Fetch : IDeploymentTask
    {
        private readonly string url;
        private readonly string destination;
        private readonly IZipExtractor extractor;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IObserver<double> progressObserver;

        public Fetch(string url, string destination, IZipExtractor extractor,
            IFileSystemOperations fileSystemOperations, IObserver<double> progressObserver)
        {
            this.url = url;
            this.destination = destination;
            this.extractor = extractor;
            this.fileSystemOperations = fileSystemOperations;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                return;
            }

            var stream = await Http.GetStream(url, progressObserver);
            await extractor.ExtractToFolder(stream, destination);
        }
    }
}