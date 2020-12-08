using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class UnzipRoot : UnzipBase
    {
        public UnzipRoot(IZipExtractor extractor, IDownloader downloader, IOperationProgress progress, IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(extractor, downloader, progress, fileSystemOperations, operationContext)
        {
        }

        protected override Task Extract(Stream stream, string finalDir)
        {
            return Extractor.ExtractRoot(stream, finalDir, Progress);
        }
    }
}