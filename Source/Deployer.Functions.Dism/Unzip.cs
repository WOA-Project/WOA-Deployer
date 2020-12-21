using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions
{
    public class Unzip : UnzipBase
    {
        public Unzip(IZipExtractor extractor, IDownloader downloader, IOperationProgress progress, IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(extractor, downloader, progress, fileSystemOperations, operationContext)
        {
        }

        protected override Task Extract(Stream stream, string finalDir)
        {
            return Extractor.Extract(stream, finalDir, Progress);
        }
    }
}